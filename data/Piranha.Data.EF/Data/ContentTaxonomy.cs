/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Newtonsoft.Json;

namespace Piranha.Data;

[Serializable]
public sealed class ContentTaxonomy
{
    /// <summary>
    /// Gets/sets the content id.
    /// </summary>
    public Guid ContentId { get; set; }

    /// <summary>
    /// Gets/sets the taxonomy id.
    /// </summary>
    public Guid TaxonomyId { get; set; }

    /// <summary>
    /// Gets/sets the content.
    /// </summary>
    [JsonIgnore]
    public Content Content { get; set; }

    /// <summary>
    /// Gets/sets the taxonomy.
    /// </summary>
    public Taxonomy Taxonomy { get; set; }
}
