/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Indexer;
using Piranha.Extend;
using Piranha.Models;

namespace Piranha.Local.Services
{
    /// <summary>
    /// The identity module.
    /// </summary>
    public class IndexEngineSearchService : ISearch
    {
        /// <summary>
        /// Gets the index engine instance.
        /// </summary>
        /// <value></value>
        public IndexEngine Engine { get; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="filename">The db filename</param>
        /// <param name="apiKey">The admin api key</param>
        public IndexEngineSearchService(string filename = "idx.db")
        {
            Engine = new IndexEngine(filename);
        }

        /// <summary>
        /// Creates or updates the searchable content for the
        /// given page.
        /// </summary>
        /// <param name="page">The page</param>
        public async Task SavePageAsync(PageBase page)
        {
            if (page.Published.HasValue)
            {
                var body = new StringBuilder();

                foreach (var block in page.Blocks)
                {
                    if (block is ISearchable searchableBlock)
                    {
                        body.AppendLine(searchableBlock.GetIndexedContent());
                    }
                }

                var cleanHtml = new Regex("<[^>]*(>|$)");
                var cleanSpaces = new Regex("[\\s\\r\\n]+");

                var cleaned = cleanSpaces.Replace(cleanHtml.Replace(body.Replace("-", " ").ToString(), " "), " ").Trim();

                var doc = new Document(
                    page.Id.ToString(),
                    page.Title,
                    page.MetaDescription,
                    page.Slug,
                    "page",
                    "Piranha.Local.IndexEngine",
                    Encoding.UTF8.GetBytes(cleaned)
                );
                await Engine.AddAsync(doc).ConfigureAwait(false);
            }
            else
            {
                Engine.DeleteDocumentByGuid(page.Id.ToString());
            }
        }

        /// <summary>
        /// Deletes the given page from the search index.
        /// </summary>
        /// <param name="page">The page to delete</param>
        public Task DeletePageAsync(PageBase page)
        {
            return Task.Run(() =>
            {
                Engine.DeleteDocumentByGuid(page.Id.ToString());
            });
        }

        /// <summary>
        /// Creates or updates the searchable content for the
        /// given post.
        /// </summary>
        /// <param name="post">The post</param>
        public async Task SavePostAsync(PostBase post)
        {
            if (post.Published.HasValue)
            {
                var body = new StringBuilder();

                foreach (var block in post.Blocks)
                {
                    if (block is ISearchable searchableBlock)
                    {
                        body.AppendLine(searchableBlock.GetIndexedContent());
                    }
                }

                var cleanHtml = new Regex("<[^>]*(>|$)");
                var cleanSpaces = new Regex("[\\s\\r\\n]+");

                var cleaned = cleanSpaces.Replace(cleanHtml.Replace(body.Replace("-", " ").ToString(), " "), " ").Trim();

                var doc = new Document(
                    post.Id.ToString(),
                    post.Title,
                    post.MetaDescription,
                    post.Slug,
                    "post",
                    "Piranha.Local.IndexEngine",
                    Encoding.UTF8.GetBytes(cleaned)
                );
                await Engine.AddAsync(doc).ConfigureAwait(false);
            }
            else
            {
                Engine.DeleteDocumentByGuid(post.Id.ToString());
            }
        }

        /// <summary>
        /// Deletes the given post from the search index.
        /// </summary>
        /// <param name="post">The post to delete</param>
        public Task DeletePostAsync(PostBase post)
        {
            return Task.Run(() =>
            {
                Engine.DeleteDocumentByGuid(post.Id.ToString());
            });
        }
    }
}