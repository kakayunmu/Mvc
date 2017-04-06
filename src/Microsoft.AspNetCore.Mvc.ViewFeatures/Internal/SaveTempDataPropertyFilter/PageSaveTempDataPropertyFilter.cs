// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures.Internal
{
    public class PageSaveTempDataPropertyFilter : SaveTempDataPropertyFilterBase
    {
        public PageSaveTempDataPropertyFilter(ITempDataDictionaryFactory factory)
            : base(factory)
        {
        }

        public PageSaveTempDataPropertyFilterFactory FilterFactory { get; set; }

        public void SetTempDataProperties(Type modelType)
        {
            TempDataProperties = FilterFactory.GetTempDataProperties(modelType);
        }

        /// <summary>
        /// Applies values from TempData from <paramref name="httpContext"/> to the .
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext"/> used to find TempData.</param>
        public void ApplyTempDataChanges(HttpContext httpContext)
        {
            if (Subject == null)
            {
                throw new ArgumentNullException(nameof(Subject));
            }

            var tempData = _factory.GetTempData(httpContext);

            if (OriginalValues == null)
            {
                OriginalValues = new Dictionary<PropertyInfo, object>();
            }

            SetPropertyVaules(tempData, Subject);
        }
    }
}
