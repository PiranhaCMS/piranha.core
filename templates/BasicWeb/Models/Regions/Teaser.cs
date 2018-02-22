using Piranha.AttributeBuilder;
using Piranha.Extend.Fields;
using Piranha.Models;

namespace BasicWeb.Models.Regions
{
    /// <summary>
    /// Simple region for a teaser.
    /// </summary>
    public class Teaser
    {
        /// <summary>
        /// Gets/sets the main title.
        /// </summary>
        [Field(Options = FieldOption.HalfWidth)]
        public StringField Title { get; set; }

        /// <summary>
        /// Gets/sets the optional subtitle.
        /// </summary>
        [Field(Options = FieldOption.HalfWidth)]
        public StringField SubTitle { get; set; }

        /// <summary>
        /// Gets/sets the optional page link.
        /// </summary>
        [Field(Title = "Optional Page link")]
        public PageField PageLink { get; set; }

        /// <summary>
        /// Gets/sets the optional post link.
        /// </summary>
        [Field(Title = "Optional Post link")]
        public PostField PostLink { get; set; }

        /// <summary>
        /// Gets/sets the main body.
        /// </summary>
        [Field]
        public HtmlField Body { get; set; }

        public Teaser() {
            PageLink = new PageField();
            PostLink = new PostField();
        }
    }
}
