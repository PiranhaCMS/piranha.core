using Piranha.Data.RavenDb.Data;
using Raven.Client.Documents.Indexes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;


namespace Piranha.Data.RavenDb.Indexes;

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
                          let type = LoadDocument<Piranha.Models.ContentType>(c.TypeId)
                          select new IndexEntry
                          {
                              Group = type != null ? type.Group : null,
                              UseBlocks = type != null ? type.UseBlocks : false,
                              UseCategory = type != null ? type.UseCategory : false,
                              UseTags = type != null ? type.UseTags : false
                          };
    }
}
