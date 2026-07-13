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

/// <summary>
/// Content field for a block.
/// </summary>
[Serializable]
public sealed class BlockField : BlockFieldBase, ITranslatable
{
    /// <summary>
    /// Gets/sets the block containing the field.
    /// </summary>
    [JsonIgnore]
    public Block Block { get; set; }

    /// <summary>
    /// Gets/sets the available translations.
    /// </summary>
    public IList<PageBlockFieldTranslation> Translations { get; set; } = new List<PageBlockFieldTranslation>();

    /// <summary>
    /// Sets the translation for the specified language.
    /// </summary>
    public void SetTranslation(Guid parentId, Guid languageId, object model)
    {
        var translation = Translations.FirstOrDefault(t => t.LanguageId == languageId);

        if (translation == null)
        {
            translation = new PageBlockFieldTranslation
            {
                FieldId = parentId,
                LanguageId = languageId
            };
            Translations.Add(translation);
        }
        translation.Value = App.SerializeObject(model, model.GetType());
    }

    /// <summary>
    /// Gets the translation for the specified language.
    /// </summary>
    public object GetTranslation(Guid languageId)
    {
        return Translations.FirstOrDefault(t => t.LanguageId == languageId)?.Value;
    }
}
