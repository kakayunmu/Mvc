// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures.Internal
{
    public class SaveTempDataPropertyFilterTestBase
    {
        protected IList<TempDataProperty> BuildPropertyHelpers<TSubject>()
        {
            var subjectType = typeof(TSubject);

            var properties = subjectType.GetProperties(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            var result = new List<TempDataProperty>();

            foreach (var property in properties)
            {
                result.Add(new TempDataProperty(property, property.GetValue, property.SetValue));
            }

            return result;
        }
    }

    public class ControllerSaveTempDataPropertyFilterTest : SaveTempDataPropertyFilterTestBase
    {
        [Fact]
        public void PopulatesTempDataWithValuesFromControllerProperty()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
            {
                ["TempDataProperty-Test"] = "FirstValue"
            };

            var filter = CreateControllerSaveTempDataPropertyFilter(httpContext, tempData);

            var controller = new TestController();

            filter.TempDataProperties = BuildPropertyHelpers<TestController>();
            var context = new ActionExecutingContext(
                new ActionContext
                {
                    HttpContext = httpContext,
                    RouteData = new RouteData(),
                    ActionDescriptor = new ActionDescriptor(),
                },
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                controller);

            // Act
            filter.OnActionExecuting(context);
            controller.Test = "SecondValue";
            filter.OnTempDataSaving(tempData);

            // Assert
            Assert.Equal("SecondValue", controller.Test);
            Assert.Equal("SecondValue", tempData["TempDataProperty-Test"]);
            Assert.Equal(0, controller.Test2);
        }

        [Fact]
        public void ReadsTempDataFromTempDataDictionary()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
            {
                ["TempDataProperty-Test"] = "FirstValue"
            };

            var filter = CreateControllerSaveTempDataPropertyFilter(httpContext, tempData: tempData);
            var controller = new TestController();

            filter.TempDataProperties = BuildPropertyHelpers<TestController>();

            var context = new ActionExecutingContext(
                new ActionContext
                {
                    HttpContext = httpContext,
                    RouteData = new RouteData(),
                    ActionDescriptor = new ActionDescriptor(),
                },
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                controller);

            // Act
            filter.OnActionExecuting(context);
            filter.OnTempDataSaving(tempData);

            // Assert
            Assert.Equal("FirstValue", controller.Test);
            Assert.Equal(0, controller.Test2);
        }

        private ControllerSaveTempDataPropertyFilter CreateControllerSaveTempDataPropertyFilter(
            HttpContext httpContext,
            TempDataDictionary tempData)
        {
            var factory = new Mock<ITempDataDictionaryFactory>();
            factory.Setup(f => f.GetTempData(httpContext))
                .Returns(tempData);

            return new ControllerSaveTempDataPropertyFilter(factory.Object);
        }

        public class TestControllerStrings : Controller
        {
            [TempData]
            public string Test { get; set; }

            [TempData]
            public string Test2 { get; set; }
        }

        public class TestController : Controller
        {
            [TempData]
            public string Test { get; set; }

            [TempData]
            public int Test2 { get; set; }
        }
    }
}
