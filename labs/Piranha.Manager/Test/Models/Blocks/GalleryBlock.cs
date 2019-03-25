using Piranha.Extend;
using Piranha.Extend.Blocks;
using Piranha.Extend.Fields;

namespace MvcWeb.Models.Blocks
{
    [BlockGroupType(Name = "Gallery", Category = "Groups", Icon = "fas fa-images")]
    [BlockItemType(Type = typeof(ImageBlock))]
    public class GalleryBlock : BlockGroup
    {
        /// <summary>
        /// Main gallery title.
        /// </summary>
        public StringField Title { get; set; }

        /// <summary>
        /// Optional description.
        /// </summary>
        public HtmlField Description { get; set; }
    }
}