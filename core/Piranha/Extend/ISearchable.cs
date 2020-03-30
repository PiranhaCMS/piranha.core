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
    /// Interface for marking a block or field as searchable.
    /// </summary>
    public interface ISearchable
    {
        /// <summary>
        /// Gets the content that should be indexed for searching.
        /// </summary>
        string GetIndexedContent();
    }
}
