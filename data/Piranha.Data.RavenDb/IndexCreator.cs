using Piranha.Data.RavenDb.Indexes;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Piranha.Data.RavenDb;

public static class IndexCreator
{
    public static void CreateIndexes(IDocumentStore store)
    {
        // 🔥 Register all RavenDB indexes in this assembly
        IndexCreation.CreateIndexes(typeof(Revisions_ByIsNewerThanPost).Assembly, store);
    }
}