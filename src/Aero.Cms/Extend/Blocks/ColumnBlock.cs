

using System.Text;
using Aero.Cms.Models;

namespace Aero.Cms.Extend.Blocks;

/// <summary>
/// Single column quote block.
/// </summary>
[BlockGroupType(Name = "Columns", Category = "Content", Icon = "fas fa-columns", Display = BlockDisplayMode.Horizontal)]
public class ColumnBlock : BlockGroup, ISearchable
{
    /// <summary>
    /// Gets the content that should be indexed for searching.
    /// </summary>
    public string GetIndexedContent()
    {
        var content = new StringBuilder();

        foreach (var item in Items)
        {
            if (item is ISearchable searchItem)
            {
                var value = searchItem.GetIndexedContent();

                if (!string.IsNullOrEmpty(value))
                {
                    content.AppendLine(value);
                }
            }
        }
        return content.ToString();
    }
}
