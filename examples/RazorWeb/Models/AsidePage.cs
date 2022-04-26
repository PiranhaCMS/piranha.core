using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Models;

namespace RazorWeb.Models
{
    [PageType(Title = "Aside page")]
    public class AsidePage  : Page<StandardPage>
    {
        [Section(Title = "The Big Content")]
        public override IList<Block> Blocks { get; set; }

        [Section(Title = "The Small Aside")]
        public IList<Block> Aside { get; set; }
    }
}