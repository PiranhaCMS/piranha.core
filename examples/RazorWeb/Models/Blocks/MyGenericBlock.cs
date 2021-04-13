using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Extend.Fields.Settings;

namespace RazorWeb.Models.Blocks
{
    [BlockType(Name = "Generic Block", Category = "My Category", Icon = "fas fa-fish", IsGeneric = true, ListTitle = "Title")]
    public class MyGenericBlock : Block
    {
        [Field(Options = Piranha.Models.FieldOption.HalfWidth)]
        [StringFieldSettings(MaxLength = 16)]
        public StringField Title { get; set; }

        [Field(Options = Piranha.Models.FieldOption.HalfWidth)]

        public ImageField Image { get; set; }

        [Field]
        public TextField Body { get; set; }
    }
}