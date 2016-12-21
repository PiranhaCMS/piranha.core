using Piranha.Builder.Attribute;
using Piranha.Models;
using Piranha.Extend.Fields;

namespace Blog.Models
{
    [PageType(Id = "Content", Title = "Content page")]
    public class ContentPageModel : Page<ContentPageModel>
    {
        [Region]
        public MarkdownField Body { get; set; }
        [Region]
        public TextField Ingress { get; set; }
    }
}
