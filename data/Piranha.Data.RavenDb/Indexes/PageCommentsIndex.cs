using Piranha.Data.RavenDb.Data;
using Raven.Client.Documents.Indexes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Piranha.Data.RavenDb.Indexes;

public class Revisions_ByIsNewerThanPost
    : AbstractIndexCreationTask<PostRevision, Revisions_ByIsNewerThanPost.Result>
{
    public class Result
    {
        public string BlogId { get; set; }
        public bool IsNewer { get; set; }
        public string Id { get; set; }
    }

    public Revisions_ByIsNewerThanPost()
    {
        Map = revisions => from r in revisions
                           select new
                           {
                               BlogId = r.Post.BlogId,
                               IsNewer = r.Created > r.Post.LastModified,
                               Id = r.Id
                           };
    }
}