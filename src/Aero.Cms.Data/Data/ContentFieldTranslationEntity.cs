

namespace Aero.Cms.Data.Data;

[Serializable]
public sealed class ContentFieldTranslation
{
    /// <summary>
    /// Gets/sets the unique id.
    /// </summary>
    public string FieldId { get; set; }

    /// <summary>
    /// Gets/sets the language id.
    /// </summary>
    public string LanguageId { get; set; }

    /// <summary>
    /// Gets/sets the serialized value.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Gets/sets the field.
    /// </summary>
    public ContentField Field { get; set; }

    /// <summary>
    /// Gets/sets the language.
    /// </summary>
    public Language Language { get; set; }
}
