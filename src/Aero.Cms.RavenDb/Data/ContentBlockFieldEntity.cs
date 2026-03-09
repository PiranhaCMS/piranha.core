

using System.Text.Json.Serialization;

namespace Aero.Cms.RavenDb.Data;

/// <summary>
/// Content field for a block.
/// </summary>
[Serializable]
public class ContentBlockField : BlockFieldBase, ITranslatable
{
    /// <summary>
    /// Gets/sets the block containing the field.
    /// </summary>
    [JsonIgnore]
    public ContentBlock Block { get; set; }

    /// <summary>
    /// Gets/sets the available translations.
    /// </summary>
    public List<ContentBlockFieldTranslation> Translations { get; set; } = new List<ContentBlockFieldTranslation>();

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
            translation = new ContentBlockFieldTranslation
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
