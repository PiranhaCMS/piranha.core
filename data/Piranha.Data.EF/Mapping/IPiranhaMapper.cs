/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Data.EF;

/// <summary>
/// Defines the mapping contract used by the EF data layer.
/// Replaces the AutoMapper IMapper dependency.
/// </summary>
public interface IPiranhaMapper
{
    /// <summary>
    /// Maps from <typeparamref name="TSource"/> into an existing
    /// <typeparamref name="TDest"/> instance, overwriting mapped properties.
    /// </summary>
    void Map<TSource, TDest>(TSource source, TDest dest);

    /// <summary>
    /// Maps a <see cref="Data.ContentTranslation"/> onto an existing
    /// <see cref="Models.GenericContent"/> instance (title / excerpt / lastModified).
    /// </summary>
    void MapTranslation(Data.ContentTranslation source, Models.GenericContent dest);
}
