// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.AspNetCore.Razor.Evolution;
using Microsoft.AspNetCore.Razor.Evolution.Intermediate;

namespace Microsoft.AspNetCore.Mvc.Razor.Host
{
    public class PageDirective
    {
        public static readonly DirectiveDescriptor DirectiveDescriptor = DirectiveDescriptorBuilder
            .Create("page")
            .BeginOptionals()
            .AddString() // "route-template"
            .AddString() // "page-name"
            .Build();

        private PageDirective(string routeTemplate, string name)
        {
            RouteTemplate = routeTemplate;
            Name = name;
        }

        public string RouteTemplate { get; }

        public string Name { get; }

        public static IRazorEngineBuilder Register(IRazorEngineBuilder builder)
        {
            builder.AddDirective(DirectiveDescriptor);
            return builder;
        }

        public static bool TryGetPageDirective(DocumentIRNode irDocument, out PageDirective directive)
        {
            var visitor = new Visitor();
            for (var i = 0; i < irDocument.Children.Count; i++)
            {
                visitor.Visit(irDocument.Children[i]);
            }

            directive = new PageDirective(visitor.RouteTemplate, visitor.Name);
            return visitor.DirectiveNode != null;
        }

        private class Visitor : RazorIRNodeWalker
        {
            public DirectiveIRNode DirectiveNode { get; private set; }

            public string RouteTemplate { get; private set; }

            public string Name { get; private set; }

            public override void VisitDirective(DirectiveIRNode node)
            {
                if (node.Descriptor == DirectiveDescriptor)
                {
                    DirectiveNode = node;
                    var tokens = node.Tokens.ToList();
                    if (tokens.Count > 0)
                    {
                        RouteTemplate = tokens[0].Content.Trim('"');
                    }
                    
                    if (tokens.Count > 1)
                    {
                        Name = tokens[1].Content.Trim('"');
                    }
                }
            }
        }
    }
}
