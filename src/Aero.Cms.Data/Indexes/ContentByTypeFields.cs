using Aero.Cms.Data.Data;
using Aero.Cms.Models;
using Raven.Client.Documents.Indexes;


namespace Aero.Cms.Data.Indexes;

//// Strongly-typed query via IndexEntry projection
//var contents = await session
//    .Query<Content_ByTypeFields.IndexEntry, Content_ByTypeFields>()
//    .Where(e => e.Group == groupId && e.UseTags)
//    .OfType<Content>()  // cast back to the document type
//    .ToListAsync();

public class Content_ByTypeFields : AbstractIndexCreationTask<Content>
{
    public class IndexEntry
    {
        public string Group { get; set; }
        public bool UseBlocks { get; set; }
        public bool UseCategory { get; set; }
        public bool UseTags { get; set; }
    }

    public Content_ByTypeFields()
    {
        Map = contents => from c in contents
                          let type = LoadDocument<Aero.Cms.Models.ContentType>(c.TypeId)
                          select new IndexEntry
                          {
                              Group = type != null ? type.Group : null,
                              UseBlocks = type != null ? type.UseBlocks : false,
                              UseCategory = type != null ? type.UseCategory : false,
                              UseTags = type != null ? type.UseTags : false
                          };
    }
}
