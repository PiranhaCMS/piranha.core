/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Markdig;

namespace Piranha.Extend
{
    /// <summary>
    /// Interface for converting markdown to Html.
    /// </summary>
    public class DefaultMarkdown : IMarkdown
    {
        /// <summary>
        /// Gets/sets the additional pipeline to use
        /// for markdown transformation.
        /// </summary>
        public MarkdownPipeline _pipeline { get; set; }

        /// <summary>
        /// Transforms the given markdown string to html.
        /// </summary>
        /// <param name="md">The markdown</param>
        /// <returns>The transformed html</returns>
        public string Transform(string md)
        {
            if (!string.IsNullOrEmpty(md))
            {
                return Markdown.ToHtml(md, _pipeline);
            }
            return md;
        }
    }
}
