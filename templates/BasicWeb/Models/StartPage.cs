using System.Collections.Generic;
using BasicWeb.Models.Regions;
using Piranha.AttributeBuilder;
using Piranha.Extend.Fields;
using Piranha.Models;

namespace BasicWeb.Models
{
    [PageType(Title = "Start page")]
    [PageTypeRoute(Title = "Default", Route = "/start")]
    public class StartPage : Page<StartPage>
    {
        /// <summary>
        /// Gets/sets the main body.
        /// </summary>
        [Region(Title = "Main content")]
        public HtmlField Body { get; set; }

        /// <summary>
        /// Gets/sets the page heading.
        /// </summary>
        [Region]
        public Heading Heading { get; set; }

        /// <summary>
        /// Gets/sets the available teasers.
        /// </summary>
        [Region(ListTitle = "Title")]
        public IList<Teaser> Teasers { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public StartPage() {
            Teasers = new List<Teaser>();
        }
    }
}