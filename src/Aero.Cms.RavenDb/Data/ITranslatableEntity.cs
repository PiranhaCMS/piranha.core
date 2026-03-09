

namespace Aero.Cms.RavenDb.Data;

/// <summary>
/// Interface for translatable data.
/// </summary>
public interface ITranslatable
{
    /// <summary>
    /// Sets the translation for the specified language.
    /// </summary>
    /// <param name="parentId">The parent id</param>
    /// <param name="languageId">The language id</param>
    /// <param name="model">The model</param>
    void SetTranslation(string parentId, string languageId, object model);

    object GetTranslation(string languageId);
}
