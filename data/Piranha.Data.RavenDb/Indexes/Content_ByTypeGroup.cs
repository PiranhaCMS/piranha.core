using Piranha.Data.RavenDb.Data;
using Raven.Client.Documents.Indexes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Piranha.Data.RavenDb.Indexes;

public class Content_ByTypeGroup : AbstractIndexCreationTask<Content>
{
    public Content_ByTypeGroup()
    {
        Map = contents => from c in contents
                          select new
                          {
                              c.Id,
                              Type_Group = c.Type.Group,
                              Type_UseBlocks = c.Type.UseBlocks,
                              Type_UseCategory = c.Type.UseCategory
                          };

        // Optional: store fields to avoid extra document fetch
        Store(x => x.Id, FieldStorage.Yes);
    }
}