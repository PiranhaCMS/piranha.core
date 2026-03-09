using Aero.Cms.Models;
using Aero.Cms.RavenDb.Data;
using Raven.Client.Documents.Indexes;

namespace Aero.Cms.RavenDb.Indexes;

public class Content_ByTypeGroup : AbstractIndexCreationTask<Content>
{
    public class IndexEntry
    {
        public string Group { get; set; }
        public bool UseBlocks { get; set; }
        public bool UseCategory { get; set; }
    }

    public Content_ByTypeGroup()
    {
        Map = contents => from c in contents
                          let type = LoadDocument<Aero.Cms.Models.ContentType>(c.TypeId)
                          select new IndexEntry
                          {
                              Group = type != null ? type.Group : null,
                              UseBlocks = type != null ? type.UseBlocks : false,
                              UseCategory = type != null ? type.UseCategory : false
                          };
    }
}