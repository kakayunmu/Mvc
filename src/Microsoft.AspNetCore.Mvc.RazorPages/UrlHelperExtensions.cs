// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// Razor Page specific extensions for <see cref="IUrlHelper"/>.
    /// </summary>
    public static class UrlHelperExtensions
    {
        /// <summary>
        /// Generates a URL with an absolute path for the specified <paramref name="page"/>.
        /// </summary>
        /// <param name="urlHelper">The <see cref="IUrlHelper"/>.</param>
        /// <param name="page">The page to generate the url for.</param>
        /// <returns>The generated URL.</returns>
        public static string Page(this IUrlHelper urlHelper, string page)
            => Page(urlHelper, page, protocol: null);

        /// <summary>
        /// Generates a URL with an absolute path for the specified <paramref name="page"/>.
        /// </summary>
        /// <param name="urlHelper">The <see cref="IUrlHelper"/>.</param>
        /// <param name="page">The page to generate the url for.</param>
        /// <param name="protocol">The protocol for the URL, such as "http" or "https".</param>
        /// <returns>The generated URL.</returns>
        public static string Page(this IUrlHelper urlHelper, string page, string protocol)
        {
            if (urlHelper == null)
            {
                throw new ArgumentNullException(nameof(urlHelper));
            }

            var routeValueDictionary = new RouteValueDictionary
            {
                { "page", page },
            };

            return urlHelper.RouteUrl(
                routeName: null, 
                values: routeValueDictionary, 
                protocol: protocol);
        }
    }
}
