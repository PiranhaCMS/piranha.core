/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

namespace Piranha.Extend
{
    /// <summary>
    /// Interface for a markdown converter.
    /// </summary>
    public interface IMarkdown
    {
        /// <summary>
        /// Converts the given markdown string to HTML.
        /// </summary>
        /// <param name="markdown">The markdown content</param>
        /// <returns>The converted HTML content</returns>
        string ToHtml(string markdown);
    }
}