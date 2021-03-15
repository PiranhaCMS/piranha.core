using Piranha.Extend;
using Piranha.Extend.Fields;

namespace RazorWeb.Models.Blocks
{
    [BlockType(Name = "Generic Block", Category = "My Category", Icon = "fas fa-fish", IsGeneric = true, ListTitle = "Title")]
    public class MyGenericBlock : Block
    {
        public StringField Title { get; set; }
        public TextField Body { get; set; }
    }
}