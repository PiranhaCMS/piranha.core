

namespace Aero.Cms.Manager.Models;

public class StructureModel
{
    public class StructureItem
    {
        /// <summary>
        /// Gets/sets the unique page id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets/sets the available children.
        /// </summary>
        public List<StructureItem> Children { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public StructureItem()
        {
            Children = new List<StructureItem>();
        }
    }

    /// <summary>
    /// The id of the item to move.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets/sets the structure items.
    /// </summary>
    public List<StructureItem> Items { get; set; } = new List<StructureItem>();
}
