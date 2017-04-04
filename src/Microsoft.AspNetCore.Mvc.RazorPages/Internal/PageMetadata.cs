// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.Mvc.RazorPages.Internal
{
    public struct PageMetadata
    {
        public PageMetadata(
            string viewEnginePath, 
            string relativePath,
            string routePrefix)
        {
            ViewEnginePath = viewEnginePath;
            RelativePath = relativePath;
            RoutePrefix = routePrefix;
        }

        public string ViewEnginePath { get; }

        public string RelativePath { get; }

        public string RoutePrefix { get; }
    }
}
