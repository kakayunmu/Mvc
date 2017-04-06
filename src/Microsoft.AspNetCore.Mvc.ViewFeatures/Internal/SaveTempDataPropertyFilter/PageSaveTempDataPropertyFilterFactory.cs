// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures.Internal
{
    public class PageSaveTempDataPropertyFilterFactory : IFilterFactory
    {
        public IList<TempDataProperty> TempDataProperties { get; set; }

        public bool IsReusable => false;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var service = serviceProvider.GetRequiredService<PageSaveTempDataPropertyFilter>();
            service.FilterFactory = this;

            return service;
        }

        public IList<TempDataProperty> GetTempDataProperties(Type modelType)
        {
            if(TempDataProperties == null)
            {
                TempDataProperties = ControllerSaveTempDataPropertyFilterFactory.GetTempDataPropertyHelpers(modelType);
            }
            
            return TempDataProperties;
        }
    }
}