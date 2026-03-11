

using System.Text.Json.Serialization;

namespace Aero.Cms.Data.Data;

[Serializable]
public sealed class ContentField : ContentFieldBase, ITranslatable
{
    /// <summary>
    /// Gets/sets the content id.
    /// </summary>
    public string ContentId { get; set; }

    /// <summary>
    /// Gets/sets the content.
    /// </summary>
    [JsonIgnore]
    public Content Content { get; set; }

    /// <summary>
    /// Gets/sets the available translations.
    /// </summary>
    public List<ContentFieldTranslation> Translations { get; set; } = new List<ContentFieldTranslation>();

    /// <summary>
    /// Sets the translation for the specified language.
    /// </summary>
    /// <param name="parentId">The parent id</param>
    /// <param name="languageId">The language id</param>
    /// <param name="model">The model</param>
    public void SetTranslation(string parentId, string languageId, object model)
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
    public object GetTranslation(string languageId)
    {
        return Translations.FirstOrDefault(t => t.LanguageId == languageId)?.Value;
    }
}
