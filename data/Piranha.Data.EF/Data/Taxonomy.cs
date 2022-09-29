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

[Serializable]
public sealed class Taxonomy : TaxonomyBase
{
    /// <summary>
    /// Gets/sets the id used for grouping.
    /// </summary>
    public string GroupId { get; set; }

    /// <summary>
    /// Gets/sets the taxonomy type.
    /// </summary>
    public TaxonomyType Type { get; set; }
}
