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

using Newtonsoft.Json;

[Serializable]
public sealed class PostFieldTranslation
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
    /// Gets/sets the field.
    /// </summary>
    [JsonIgnore]
    public PostField Field { get; set; }

    /// <summary>
    /// Gets/sets the language.
    /// </summary>
    public Language Language { get; set; }
}
