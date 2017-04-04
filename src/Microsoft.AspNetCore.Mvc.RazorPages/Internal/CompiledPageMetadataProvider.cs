// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.RazorPages.ApplicationFeature;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Mvc.RazorPages.Internal
{
    public class CompiledPageMetadataProvider : IPageMetadataProvider
    {
        private readonly ApplicationPartManager _applicationManager;
        private readonly RazorPagesOptions _pagesOptions;
        private List<PageMetadata> _compiledPageMetadata;

        public CompiledPageMetadataProvider(
            ApplicationPartManager applicationManager,
            IOptions<RazorPagesOptions> pagesOptionsAccessor)
        {
            _applicationManager = applicationManager;
            _pagesOptions = pagesOptionsAccessor.Value;
        }

        public IEnumerable<PageMetadata> EnumeratePageMetadata()
        {
            EnsureCompiledPageMetadata();

            return _compiledPageMetadata;
        }

        private void EnsureCompiledPageMetadata()
        {
            if (_compiledPageMetadata == null)
            {
                var feature = new CompiledPageInfoFeature();
                _applicationManager.PopulateFeature(feature);

                _compiledPageMetadata = new List<PageMetadata>(feature.CompiledPages.Count);

                var rootDirectory = _pagesOptions.RootDirectory;
                if (!rootDirectory.EndsWith("/", StringComparison.Ordinal))
                {
                    rootDirectory = rootDirectory + "/";
                }

                for (var i = 0; i < feature.CompiledPages.Count; i++)
                {
                    var page = feature.CompiledPages[i];
                    if (!page.Path.StartsWith(rootDirectory))
                    {
                        continue;
                    }

                    var pageMetadata = new PageMetadata(
                        GetViewEnginePath(rootDirectory, page.Path),
                        page.Path,
                        page.RoutePrefix);

                    _compiledPageMetadata.Add(pageMetadata);
                }
            }
        }

        private string GetViewEnginePath(string rootDirectory, string path)
        {
            var endIndex = path.LastIndexOf('.');
            if (endIndex == -1)
            {
                endIndex = path.Length;
            }

            // rootDirectory = "/Pages/AllMyPages/"
            // path = "/Pages/AllMyPages/Home.cshtml"
            // Result = "/Home"
            var startIndex = rootDirectory.Length - 1;

            return path.Substring(startIndex, endIndex - startIndex);
        }
    }
}
