// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures.Internal
{
    public abstract class SaveTempDataPropertyFilterBase : ISaveTempDataCallback
    {
        protected const string Prefix = "TempDataProperty-";

        protected readonly ITempDataDictionaryFactory _factory;

        public IList<TempDataProperty> TempDataProperties { get; set; }
        
        public object Subject { get; set; }

        public IDictionary<PropertyInfo, object> OriginalValues { get; set; }

        public SaveTempDataPropertyFilterBase(ITempDataDictionaryFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Puts the modified values of  into <paramref name="tempData"/>.
        /// </summary>
        /// <param name="tempData">The <see cref="ITempDataDictionary"/> to be updated.</param>
        public void OnTempDataSaving(ITempDataDictionary tempData)
        {
            if (Subject != null && OriginalValues != null)
            {
                foreach (var kvp in OriginalValues)
                {
                    var property = kvp.Key;
                    var originalValue = kvp.Value;

                    var newValue = property.GetValue(Subject);
                    if (newValue != null && !newValue.Equals(originalValue))
                    {
                        tempData[Prefix + property.Name] = newValue;
                    }
                }
            }
        }

        protected void SetPropertyVaules(ITempDataDictionary tempData, object subject)
        {
            if (TempDataProperties == null)
            {
                return;
            }

            for (var i = 0; i < TempDataProperties.Count; i++)
            {
                var property = TempDataProperties[i];
                var value = tempData[Prefix + property.PropertyInfo.Name];

                OriginalValues[property.PropertyInfo] = value;

                var propertyTypeInfo = property.PropertyInfo.PropertyType.GetTypeInfo();

                var isReferenceTypeOrNullable = !propertyTypeInfo.IsValueType || Nullable.GetUnderlyingType(property.GetType()) != null;
                if (value != null || isReferenceTypeOrNullable)
                {
                    property.SetValue(subject, value);
                }
            }
        }
    }
}
