using Piranha.Builder.Attribute;
using Piranha.Models;
using Piranha.Extend.Fields;

namespace Blog.Models
{
    [PageType(Id = "Content", Title = "Content page")]
    public class ContentPageModel : Page<ContentPageModel>
    {
        public class ContentPageSettings
        {
            [Field]
            public ImageField PrimaryImage { get; set; }
            [Field()]
            public TextField Ingress { get; set; }

        }

        [Region]
        public MarkdownField Body { get; set; }
        [Region]
        public ContentPageSettings Settings { get; set; }
    }
}
