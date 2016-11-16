using System.Collections.Generic;
using Piranha.Models;
using Piranha.Builder.Attribute;
using Piranha.Extend.Fields;

namespace Blog.Models
{
    public class TeaserItem
    {
        [Field]
        public StringField Title { get; set; }
        [Field]
        public HtmlField Body { get; set; }
    }

    [PageType(Id = "Test", Title = "Test page")]
    public class TestPageModel : Page<StartPageModel>
    {
        [Region]
        public HtmlField Content { get; set; }

        [Region(Max = 5)]
        public IList<TeaserItem> Teasers { get; set; }

        public TestPageModel() {
            Teasers = new List<TeaserItem>();
        }
    }
}
