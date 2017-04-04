using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.AspNetCore.Razor.Evolution;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Mvc.RazorPages.Internal
{
    public class RazorProjectPageMetadataProvider : IPageMetadataProvider
    {
        private readonly RazorProject _project;
        private readonly RazorPagesOptions _pagesOptions;

        public RazorProjectPageMetadataProvider(
            RazorProject razorProject,
            IOptions<RazorPagesOptions> pagesOptionsAccessor)
        {
            _project = razorProject;
            _pagesOptions = pagesOptionsAccessor.Value;
        }

        public IEnumerable<PageMetadata> EnumeratePageMetadata()
        {
            foreach (var item in _project.EnumerateItems(_pagesOptions.RootDirectory))
            {
                if (item.FileName.StartsWith("_"))
                {
                    // Pages like _PageImports should not be routable.
                    continue;
                }

                if (!PageDirectiveFeature.TryGetPageDirective(item, out var routeTemplate))
                {
                    // .cshtml pages without @page are not RazorPages.
                    continue;
                }

                yield return new PageMetadata(item.PathWithoutExtension, item.CombinedPath, routeTemplate);
            }
        }
    }
}
