// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;

namespace Microsoft.AspNetCore.Mvc.RazorPages.ApplicationFeature
{
    /// <summary>
    /// An <see cref="IApplicationFeatureProvider{TFeature}"/> for <see cref="CompiledPageInfoFeature"/>.
    /// </summary>
    public class CompiledPageInfoFeatureProvider :
        IApplicationFeatureProvider<CompiledPageInfoFeature>,
        IApplicationFeatureProvider<ViewsFeature>
    {
        /// <summary>
        /// Gets the namespace for the <see cref="ViewInfoContainer"/> type in the view assembly.
        /// </summary>
        public static readonly string CompiledPageManifestNamespace = "AspNetCore";

        /// <summary>
        /// Gets the type name for the view collection type in the view assembly.
        /// </summary>
        public static readonly string CompiledPageManifestTypeName = "__CompiledRazorPagesManifest";

        private static readonly string FullyQualifiedManifestTypeName =
            CompiledPageManifestNamespace + "." + CompiledPageManifestTypeName;

        /// <inheritdoc />
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, CompiledPageInfoFeature feature)
        {
            foreach (var manifest in GetManifests(parts))
            {
                foreach (var item in manifest.CompiledPages)
                {
                    feature.CompiledPages.Add(item);
                }
            }
        }

        /// <inheritdoc />
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ViewsFeature feature)
        {
            foreach (var manifest in GetManifests(parts))
            {
                foreach (var item in manifest.CompiledPages)
                {
                    feature.Views.Add(item.Path, item.CompiledType);
                }
            }
        }

        private static IEnumerable<CompiledPageManifest> GetManifests(IEnumerable<ApplicationPart> parts)
        {
            return parts.OfType<AssemblyPart>()
                .Select(part => CompiledViewManfiest.LoadManifest<CompiledPageManifest>(part, FullyQualifiedManifestTypeName))
                .Where(manifest => manifest != null);
        }
    }
}
