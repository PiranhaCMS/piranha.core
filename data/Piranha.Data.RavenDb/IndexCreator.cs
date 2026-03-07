using Piranha.Data.RavenDb.Indexes;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;

namespace Piranha.Data.RavenDb;

public static class IndexCreator
{
    /// <summary>
    /// Registers all static RavenDB indexes in this assembly.
    /// Uses ExecuteIndex (fire-and-forget) rather than IndexCreation.CreateIndexes
    /// to avoid the synchronous 15-second Raft-consensus timeout that occurs
    /// in embedded / test scenarios with multiple indexes.
    /// Indexes build in the background on the server side.
    /// </summary>
    public static void CreateIndexes(IDocumentStore store)
    {
        new Pages_BySite().Execute(store);
        new Posts_ByBlog().Execute(store);
        new Posts_ByTag().Execute(store);
        new PostRevisions_ByBlog().Execute(store);
        new PageRevisions_BySite().Execute(store);
        new Revisions_ByIsNewerThanPost().Execute(store);
        new Content_ByTypeGroup().Execute(store);
    }
}