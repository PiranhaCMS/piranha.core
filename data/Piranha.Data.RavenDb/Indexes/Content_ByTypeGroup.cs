using Piranha.Data.RavenDb.Data;
using Raven.Client.Documents.Indexes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Piranha.Data.RavenDb.Indexes;

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
                          let type = LoadDocument<Piranha.Models.ContentType>(c.TypeId)
                          select new IndexEntry
                          {
                              Group = type != null ? type.Group : null,
                              UseBlocks = type != null ? type.UseBlocks : false,
                              UseCategory = type != null ? type.UseCategory : false
                          };
    }
}