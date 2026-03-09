/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Data.RavenDb.Data;

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
