/*
 * Copyright (c) .NET Foundation and Contributors
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
    /// Interface for converting markdown to Html.
    /// </summary>
    public interface IMarkdown
    {
        /// <summary>
        /// Transforms the given markdown string to html.
        /// </summary>
        /// <param name="md">The markdown</param>
        /// <returns>The transformed html</returns>
        string Transform(string md);
    }
}
