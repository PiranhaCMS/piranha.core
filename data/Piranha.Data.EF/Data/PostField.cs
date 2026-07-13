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
public sealed class PostField : ContentFieldBase, ITranslatable
{
    /// <summary>
    /// Gets/sets the post id.
    /// </summary>
    public Guid PostId { get; set; }

    /// <summary>
    /// Gets/sets the post.
    /// </summary>
    [JsonIgnore]
    public Post Post { get; set; }

    /// <summary>
    /// Gets/sets the available translations.
    /// </summary>
    public IList<PostFieldTranslation> Translations { get; set; } = new List<PostFieldTranslation>();

    /// <summary>
    /// Sets the translation for the specified language.
    /// </summary>
    public void SetTranslation(Guid parentId, Guid languageId, object model)
    {
        var translation = Translations.FirstOrDefault(t => t.LanguageId == languageId);

        if (translation == null)
        {
            translation = new PostFieldTranslation
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
