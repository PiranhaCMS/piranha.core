using Piranha.AttributeBuilder;
using Piranha.Extend.Fields;
using Piranha.Models;

namespace BasicWeb.Models
{
    [PostType(Title = "Blog post")]
    public class BlogPost  : Post<BlogPost>
    {
        /// <summary>
        /// Gets/sets the main body.
        /// </summary>
        [Region(Title = "Main content")]
        public HtmlField Body { get; set; }

        /// <summary>
        /// Gets/sets the post heading.
        /// </summary>
        [Region]
        public Regions.Heading Heading { get; set; }
    }
}