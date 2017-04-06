// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures.Internal
{
    public class PageSaveTempDataPropertyFilterTest : SaveTempDataPropertyFilterTestBase
    {
        [Fact]
        public void PopulatesTempDataWithValuesFromPageProperty()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void ApplyTempDataChanges_ToPageModel_SetsPropertyValue()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();

            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
            {
                { "TempDataProperty-Test", "Value" }
            };
            tempData.Save();

            var page = new TestPageString()
            {
                ViewContext = CreateViewContext(httpContext, tempData)
            };

            var provider = CreatePageSaveTempDataPropertyFilter(httpContext, tempData: tempData);
            provider.Subject = page;
            provider.TempDataProperties = BuildPropertyHelpers<TestPageString>();

            // Act
            provider.ApplyTempDataChanges(httpContext);

            // Assert
            Assert.Equal("Value", page.Test);
            Assert.Null(page.Test2);
        }

        [Fact]
        public void ApplyTempDataChanges_ToPage_SetsPropertyValue()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();

            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
            {
                { "TempDataProperty-Test", "Value" }
            };
            tempData.Save();

            var page = new TestPageString()
            {
                ViewContext = CreateViewContext(httpContext, tempData)
            };

            var provider = CreatePageSaveTempDataPropertyFilter(httpContext, tempData: tempData);
            provider.Subject = page;
            provider.TempDataProperties = BuildPropertyHelpers<TestPageString>();

            // Act
            provider.ApplyTempDataChanges(httpContext);

            // Assert
            Assert.Equal("Value", page.Test);
            Assert.Null(page.Test2);
        }

        private static PageContext CreateViewContext(HttpContext httpContext, ITempDataDictionary tempData)
        {
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var metadataProvider = new EmptyModelMetadataProvider();
            var viewData = new ViewDataDictionary(metadataProvider, new ModelStateDictionary());
            var viewContext = new PageContext(
                actionContext,
                viewData,
                tempData,
                new HtmlHelperOptions());

            return viewContext;
        }

        private PageSaveTempDataPropertyFilter CreatePageSaveTempDataPropertyFilter(
            HttpContext httpContext, 
            TempDataDictionary tempData)
        {
            var factory = new Mock<ITempDataDictionaryFactory>();
            factory.Setup(f => f.GetTempData(httpContext))
                .Returns(tempData);

            return new PageSaveTempDataPropertyFilter(factory.Object);
        }

        public class TestPageString : Page
        {
            [TempData]
            public string Test { get; set; }

            [TempData]
            public string Test2 { get; set; }

            public override Task ExecuteAsync()
            {
                throw new NotImplementedException();
            }
        }

        public class TestPageStringWithModel : Page
        {
            public PageModel TestPageModelWithString { get; set; }

            public override Task ExecuteAsync()
            {
                throw new NotImplementedException();
            }
        }

        public class TestPageModelWithString : PageModel
        {
            [TempData]
            public string Test { get; set; }

            [TempData]
            public string Test2 { get; set; }
        }
    }
}
