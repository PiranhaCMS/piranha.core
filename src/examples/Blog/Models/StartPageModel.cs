using System.Collections.Generic;
using Piranha.Models;
using Piranha.Extend.Fields;

namespace Blog.Models
{
    public class SliderItem
    {
        public StringField Title { get; set; }
        public HtmlField Body { get; set; }
    }

    public class IntroBlock
    {
        public StringField Title { get; set; }
        public TextField Body { get; set; }
    }

    public class StartPageModel : Page<StartPageModel>
    {
        public HtmlField Content { get; set; }
        public IntroBlock Intro { get; set; }
        public IList<SliderItem> Slider { get; set; }

        public StartPageModel() {
            Slider = new List<SliderItem>();
        }
    }
}
