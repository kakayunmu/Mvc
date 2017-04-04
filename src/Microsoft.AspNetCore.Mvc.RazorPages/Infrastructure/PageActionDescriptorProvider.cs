// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages.Internal;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure
{
    public class PageActionDescriptorProvider : IActionDescriptorProvider
    {
        private static readonly string IndexFileName = "Index.cshtml";
        private readonly List<IPageMetadataProvider> _pageMetadataProviders;
        private readonly MvcOptions _mvcOptions;
        private readonly RazorPagesOptions _pagesOptions;

        public PageActionDescriptorProvider(
            IEnumerable<IPageMetadataProvider> pageMetadataProviders,
            IOptions<MvcOptions> mvcOptionsAccessor,
            IOptions<RazorPagesOptions> pagesOptionsAccessor)
        {
            _pageMetadataProviders = pageMetadataProviders.ToList();
            _mvcOptions = mvcOptionsAccessor.Value;
            _pagesOptions = pagesOptionsAccessor.Value;
        }

        public int Order { get; set; }

        public void OnProvidersExecuting(ActionDescriptorProviderContext context)
        {
            for (var i = 0; i < _pageMetadataProviders.Count; i++)
            {
                var pageMetadataProvider = _pageMetadataProviders[i];
                foreach (var item in pageMetadataProvider.EnumeratePageMetadata())
                {
                    if (AttributeRouteModel.IsOverridePattern(item.RoutePrefix))
                    {
                        throw new InvalidOperationException(string.Format(
                            Resources.PageActionDescriptorProvider_RouteTemplateCannotBeOverrideable,
                            item.RelativePath));
                    }

                    AddActionDescriptors(context.Results, item);
                }
            }
        }

        public void OnProvidersExecuted(ActionDescriptorProviderContext context)
        {
        }

        private void AddActionDescriptors(IList<ActionDescriptor> actions, PageMetadata item)
        {
            var model = new PageApplicationModel(item.RelativePath, item.ViewEnginePath);
            model.Selectors.Add(CreateSelectorModel(item.ViewEnginePath, item.RoutePrefix));

            var fileName = Path.GetFileName(item.RelativePath);
            if (string.Equals(IndexFileName, fileName, StringComparison.OrdinalIgnoreCase))
            {
                var parentDirectoryPath = item.ViewEnginePath;
                var index = parentDirectoryPath.LastIndexOf('/');
                if (index == -1)
                {
                    parentDirectoryPath = string.Empty;
                }
                else
                {
                    parentDirectoryPath = parentDirectoryPath.Substring(0, index);
                }
                model.Selectors.Add(CreateSelectorModel(parentDirectoryPath, item.RoutePrefix));
            }

            for (var i = 0; i < _pagesOptions.Conventions.Count; i++)
            {
                _pagesOptions.Conventions[i].Apply(model);
            }

            var filters = new List<FilterDescriptor>(_mvcOptions.Filters.Count + model.Filters.Count);
            for (var i = 0; i < _mvcOptions.Filters.Count; i++)
            {
                filters.Add(new FilterDescriptor(_mvcOptions.Filters[i], FilterScope.Global));
            }

            for (var i = 0; i < model.Filters.Count; i++)
            {
                filters.Add(new FilterDescriptor(model.Filters[i], FilterScope.Action));
            }

            foreach (var selector in model.Selectors)
            {
                actions.Add(new PageActionDescriptor()
                {
                    AttributeRouteInfo = new AttributeRouteInfo()
                    {
                        Name = selector.AttributeRouteModel.Name,
                        Order = selector.AttributeRouteModel.Order ?? 0,
                        Template = selector.AttributeRouteModel.Template,
                    },
                    DisplayName = $"Page: {item.ViewEnginePath}",
                    FilterDescriptors = filters,
                    Properties = new Dictionary<object, object>(model.Properties),
                    RelativePath = item.RelativePath,
                    RouteValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "page", item.ViewEnginePath},
                    },
                    ViewEnginePath = item.ViewEnginePath,
                });
            }
        }

        private static SelectorModel CreateSelectorModel(string prefix, string template)
        {
            return new SelectorModel
            {
                AttributeRouteModel = new AttributeRouteModel
                {
                    Template = AttributeRouteModel.CombineTemplates(prefix, template),
                }
            };
        }
    }
}