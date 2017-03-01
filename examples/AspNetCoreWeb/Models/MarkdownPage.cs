using Piranha.AttributeBuilder;
using Piranha.Models;
using Piranha.Extend.Fields;

namespace AspNetCoreWeb.Models
{
    [PageType(Title = "Markdown page")]
    public class MarkdownPage : Page<MarkdownPage>
    {
        [Region]
        public MarkdownField Body { get; set; }
        [Region]
        public TextField Ingress { get; set; }
    }
}
