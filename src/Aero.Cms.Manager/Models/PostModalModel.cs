

namespace Aero.Cms.Manager.Models;

public class PostModalModel
{
    public class PostModalItem
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Permalink { get; set; }
        public string Published { get; set; }
    }

    public class SiteItem
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }

    public class ArchiveItem
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
    }

    public IEnumerable<SiteItem> Sites { get; set; } = new List<SiteItem>();
    public IEnumerable<ArchiveItem> Archives { get; set; } = new List<ArchiveItem>();
    public IEnumerable<PostModalItem> Posts { get; set; } = new List<PostModalItem>();

    public string SiteId { get; set; }
    public string ArchiveId { get; set; }

    public string SiteTitle { get; set; }
    public string ArchiveTitle { get; set; }
    public string ArchiveSlug { get; set; }
}
