using Piranha.Extend;
using Piranha.Extend.Fields;

namespace rlomweb.Models.Regions
{
    public class Hero
    {
        /// <summary>
        /// Gets/sets the optional primary image.
        /// </summary>
        [Field(Title = "Primary image")]
        public ImageField PrimaryImage { get; set; }

        /// <summary>
        /// Gets/sets the optional ingress.
        /// </summary>
        [Field]
        public TextField Ingress { get; set; }
    }
}