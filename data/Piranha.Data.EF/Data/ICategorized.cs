/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Data;

/// <summary>
/// Interface for categorized content.
/// </summary>
public interface ICategorized
{
    /// <summary>
    /// Gets/sets the category id.
    /// </summary>
    Guid? CategoryId { get; set; }
}
