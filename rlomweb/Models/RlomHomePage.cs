using rlomweb.Models.Regions;
using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Models;
using System.Collections.Generic;

namespace rlomweb.Models
{
    [PageType(Title = "Rlom home page")]
    [PageTypeRoute(Title = "Default", Route = "/home")]
    public class RlomHomePage : Page<RlomHomePage>
    {

        /// <summary>
        /// Gets/sets the available teasers.
        /// </summary>s
        [Region(ListTitle = "Title")]
        public IList<Teaser> Teasers { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RlomHomePage()
        {
            Teasers = new List<Teaser>();
        }

    }
 }