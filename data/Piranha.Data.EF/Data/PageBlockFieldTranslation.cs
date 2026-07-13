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
public sealed class PageBlockFieldTranslation
{
    /// <summary>
    /// Gets/sets the field id.
    /// </summary>
    public Guid FieldId { get; set; }

    /// <summary>
    /// Gets/sets the language id.
    /// </summary>
    public Guid LanguageId { get; set; }

    /// <summary>
    /// Gets/sets the serialized value.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Gets/sets the block field.
    /// </summary>
    [JsonIgnore]
    public BlockField Field { get; set; }

    /// <summary>
    /// Gets/sets the language.
    /// </summary>
    public Language Language { get; set; }
}
