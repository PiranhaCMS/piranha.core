using BasicWeb.Models.Regions;
using Piranha.AttributeBuilder;
using Piranha.Models;

namespace BasicWeb.Models
{
    [PageType(Title = "Blog archive")]
    public class BlogArchive  : BlogPage<BlogArchive>
    {
        /// <summary>
        /// Gets/sets the archive heading.
        /// </summary>
        [Region]
        public Heading Heading { get; set; }
    }
}