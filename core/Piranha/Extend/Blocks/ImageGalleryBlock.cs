/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Extend.Blocks;

/// <summary>
/// Image gallery block.
/// </summary>
[BlockGroupType(Name = "Gallery", Category = "Media", Icon = "fas fa-images")]
[BlockItemType(Type = typeof(ImageBlock))]
public class ImageGalleryBlock : BlockGroup { }
