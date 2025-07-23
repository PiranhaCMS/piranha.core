/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Azure;

/// <summary>
/// How uploaded media files should be named to
/// ensure unique paths.
/// </summary>
public enum BlobStorageNaming
{
    UniqueFileNames,
    UniqueFolderNames
}
