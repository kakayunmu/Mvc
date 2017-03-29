// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Mvc.Razor.Host;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Razor.Evolution;

namespace Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure
{
    public static class PageDirectiveFeature
    {
        public static bool TryGetPageDirective(
            RazorTemplateEngine templateEngine, 
            RazorProjectItem projectItem,
            out PageDirective directive)
        {
            if (templateEngine == null)
            {
                throw new ArgumentNullException(nameof(templateEngine));
            }

            if (projectItem == null)
            {
                throw new ArgumentNullException(nameof(projectItem));
            }

            var codeDocument = templateEngine.CreateCodeDocument(projectItem);
            templateEngine.Engine.Process(codeDocument);
            directive = codeDocument.GetPageDirective();
            return directive != null;
        }
    }
}
