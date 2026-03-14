

namespace Aero.Cms.Extend.Blocks;

/// <summary>
/// Image gallery block.
/// </summary>
[BlockGroupType(Name = "Gallery", Category = "Media", Icon = "fas fa-images")]
[BlockItemType(Type = typeof(ImageBlock))]
public class ImageGalleryBlock : BlockGroup { }
