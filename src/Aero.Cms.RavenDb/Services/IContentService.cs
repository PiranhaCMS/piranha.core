

using Aero.Cms.RavenDb.Data;
using Aero.Cms.RavenDb.Data;

namespace Aero.Cms.RavenDb.Services;

/// <summary>
/// Service for transforming between data entities and
/// content models.
/// </summary>
/// <typeparam name="TContent">The content type</typeparam>
/// <typeparam name="TField">The field type</typeparam>
/// <typeparam name="TModelBase">The content base type</typeparam>
public interface IContentService<TContent, TField, TModelBase>
    where TContent : ContentBase<TField>
    where TField : ContentFieldBase
    where TModelBase : Models.ContentBase
{
    /// <summary>
    /// Transforms the given data into a new model.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    /// <param name="content">The content entity</param>
    /// <param name="type">The content type</param>
    /// <param name="process">Optional func that should be called after transformation</param>
    /// <param name="languageId">The optional language id</param>
    /// <returns>The page model</returns>
    Task<T> TransformAsync<T>(TContent content, Models.ContentTypeBase type, Func<TContent, T, Task> process = null, string? languageId = null)
        where T : Models.ContentBase, TModelBase;

    /// <summary>
    /// Transforms the given model into content data.
    /// </summary>
    /// <param name="model">The model</param>
    /// <param name="type">The conten type</param>
    /// <param name="dest">The optional dest object</param>
    /// <param name="languageId">The optional language id</param>
    /// <returns>The content data</returns>
    TContent Transform<T>(T model, Models.ContentTypeBase type, TContent dest = null, string? languageId = null)
        where T : Models.ContentBase, TModelBase;

    /// <summary>
    /// Transforms the given block data into block models.
    /// </summary>
    /// <param name="blocks">The data</param>
    /// <returns>The transformed blocks</returns>
    List<Extend.Block> TransformBlocks(IEnumerable<Block> blocks);

    /// <summary>
    /// Transforms the given block data into block models.
    /// </summary>
    /// <param name="blocks">The data</param>
    /// <param name="languageId">The language id</param>
    /// <returns>The transformed blocks</returns>
    List<Extend.Block> TransformBlocks(IEnumerable<ContentBlock> blocks, string? languageId);

    /// <summary>
    /// Transforms the given blocks to the internal data model.
    /// </summary>
    /// <param name="models">The blocks</param>
    /// <returns>The data model</returns>
    List<Block> TransformBlocks(List<Extend.Block> models);

    /// <summary>
    /// Transforms the given blocks to the internal data model.
    /// </summary>
    /// <param name="models">The blocks</param>
    /// <param name="languageId">The current language</param>
    /// <returns>The data model</returns>
    List<ContentBlock> TransformContentBlocks(List<Extend.Block> models, string languageId);
}
