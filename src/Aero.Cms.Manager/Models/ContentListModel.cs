

using Aero.Cms.Manager.Models.Content;
using Aero.Cms.Models;

namespace Aero.Cms.Manager.Models;

/// <summary>
/// Content list model.
/// </summary>
public class ContentListModel
{
    public class ContentItem
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string TypeId { get; set; }
        public string Modified { get; set; }
        public string Status { get; set; }
        public string EditUrl { get; set; }
    }

    public ContentGroup Group { get; set; }
    public IEnumerable<ContentGroup> Groups { get; set; } = new List<ContentGroup>();
    public IEnumerable<ContentItem> Items { get; set; } = new List<ContentItem>();
    public List<ContentTypeModel> Types { get; set; } = new List<ContentTypeModel>();
    public StatusMessage Status { get; set; }
}
