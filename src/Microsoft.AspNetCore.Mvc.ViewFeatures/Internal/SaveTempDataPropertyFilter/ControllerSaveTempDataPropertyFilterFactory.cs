// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures.Internal
{
    public class ControllerSaveTempDataPropertyFilterFactory : IFilterFactory
    {
        // Cannot be public as <c>PropertyHelper</c> is an internal shared source type
        public IList<TempDataProperty> TempDataProperties { get; set; }

        public bool IsReusable => false;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var service = serviceProvider.GetRequiredService<ControllerSaveTempDataPropertyFilter>();
            service.TempDataProperties = TempDataProperties;
            return service;
        }

        public static IList<TempDataProperty> GetTempDataPropertyHelpers(Type modelType)
        {
            IList<TempDataProperty> results = null;

            var propertyHelpers = PropertyHelper.GetVisibleProperties(type: modelType);

            for (var i = 0; i < propertyHelpers.Length; i++)
            {
                var propertyHelper = propertyHelpers[i];
                if (propertyHelper.Property.IsDefined(typeof(TempDataAttribute)))
                {
                    ValidateProperty(propertyHelper);
                    if (results == null)
                    {
                        results = new List<TempDataProperty>();
                    }

                    results.Add(new TempDataProperty(
                        propertyHelper.Property,
                        propertyHelper.GetValue,
                        propertyHelper.SetValue));
                }
            }

            return results;
        }

        private static void ValidateProperty(PropertyHelper propertyHelper)
        {
            var property = propertyHelper.Property;
            if (!(property.SetMethod != null &&
                property.SetMethod.IsPublic &&
                property.GetMethod != null &&
                property.GetMethod.IsPublic))
            {
                throw new InvalidOperationException(
                    Resources.FormatTempDataProperties_PublicGetterSetter(property.DeclaringType.FullName, property.Name, nameof(TempDataAttribute)));
            }

            if (!(property.PropertyType.GetTypeInfo().IsPrimitive || property.PropertyType == typeof(string)))
            {
                throw new InvalidOperationException(
                    Resources.FormatTempDataProperties_PrimitiveTypeOrString(property.DeclaringType.FullName, property.Name, nameof(TempDataAttribute)));
            }
        }
    }
}
