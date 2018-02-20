using Piranha.AttributeBuilder;
using Piranha.Extend.Fields;
using Piranha.Models;

namespace BasicWeb.Models
{
    [PageType(Title = "Standard page")]
    public class StandardPage  : Page<StandardPage>
    {
        /// <summary>
        /// Gets/sets the main body.
        /// </summary>
        [Region(Title = "Main content")]
        public HtmlField Body { get; set; }
    }
}