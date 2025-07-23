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
public sealed class ContentField : ContentFieldBase, ITranslatable
{
    /// <summary>
    /// Gets/sets the content id.
    /// </summary>
    public Guid ContentId { get; set; }

    /// <summary>
    /// Gets/sets the content.
    /// </summary>
    [JsonIgnore]
    public Content Content { get; set; }

    /// <summary>
    /// Gets/sets the available translations.
    /// </summary>
    public IList<ContentFieldTranslation> Translations { get; set; } = new List<ContentFieldTranslation>();

    /// <summary>
    /// Sets the translation for the specified language.
    /// </summary>
    /// <param name="parentId">The parent id</param>
    /// <param name="languageId">The language id</param>
    /// <param name="model">The model</param>
    public void SetTranslation(Guid parentId, Guid languageId, object model)
    {
        var translation = Translations.FirstOrDefault(t => t.LanguageId == languageId);

        if (translation == null)
        {
            translation = new ContentFieldTranslation
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
    /// <param name="languageId">The language id</param>
    /// <returns>The translation</returns>
    public object GetTranslation(Guid languageId)
    {
        return Translations.FirstOrDefault(t => t.LanguageId == languageId)?.Value;
    }
}
