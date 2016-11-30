/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using HeyRed.MarkdownSharp;

namespace Piranha.Extend
{
    /// <summary>
    /// Markdown implementation with MarkdownSharp.
    /// </summary>
    public class MarkdownSharp : IMarkdown
    {
        #region Members
        /// <summary>
        /// The private markdown converter.
        /// </summary>
        private readonly Markdown converter;
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MarkdownSharp() {
            converter = new Markdown();
        }

        /// <summary>
        /// Converts the given markdown string to HTML.
        /// </summary>
        /// <param name="markdown">The markdown content</param>
        /// <returns>The converted HTML content</returns>
        public string ToHtml(string markdown) {
            return converter.Transform(markdown);
        }
    }
}
