/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Piranha.Extend.Blocks;
using Piranha.Models;

namespace Piranha.Azure.Search
{
    /// <summary>
    /// The identity module.
    /// </summary>
    public static class ContentSearch
    {
        private static string searchServiceName = "";
        private static string adminApiKey = "";

        public static void CreateIndexes()
        {
            using (var client = CreateClient())
            {
                var contentIndex = new Index()
                {
                    Name = "content",
                    Fields = FieldBuilder.BuildForType<Content>()
                };

                client.Indexes.CreateOrUpdate(contentIndex);
            }
        }

        public static void PageSave(PageBase page)
        {
            using (var client = CreateClient())
            {
                var indexClient = client.Indexes.GetClient("content");
                var body = new StringBuilder();

                foreach (var block in page.Blocks)
                {
                    if (block is HtmlBlock htmlBlock)
                    {
                        body.AppendLine(htmlBlock.Body.Value);
                    }
                    else if (block is HtmlColumnBlock columnBlock)
                    {
                        body.AppendLine(columnBlock.Column1.Value);
                        body.AppendLine(columnBlock.Column2.Value);
                    }
                }

                var cleanHtml = new Regex("<[^>]*(>|$)");
                var cleanSpaces = new Regex("[\\s\\r\\n]+");

                var cleaned = cleanSpaces.Replace(cleanHtml.Replace(body.ToString(), " "), " ").Trim();

                var actions = new IndexAction<Content>[]
                {
                    IndexAction.MergeOrUpload(
                        new Content
                        {
                            Slug = page.Slug,
                            ContentId = page.Id.ToString(),
                            ContentType = "page",
                            Title = page.Title,
                            Body = cleaned
                        }
                    )
                };
                var batch = IndexBatch.New(actions);

                indexClient.Documents.Index(batch);
            }
        }

        public static void PageDelete(PageBase page)
        {

        }

        public static void PostSave(PostBase post)
        {
            using (var client = CreateClient())
            {
                var indexClient = client.Indexes.GetClient("content");
                var body = new StringBuilder();

                foreach (var block in post.Blocks)
                {
                    if (block is HtmlBlock htmlBlock)
                    {
                        body.AppendLine(htmlBlock.Body.Value);
                    }
                    else if (block is HtmlColumnBlock columnBlock)
                    {
                        body.AppendLine(columnBlock.Column1.Value);
                        body.AppendLine(columnBlock.Column2.Value);
                    }
                }

                var cleanHtml = new Regex("<[^>]*(>|$)");
                var cleanSpaces = new Regex("[\\s\\r\\n]+");

                var cleaned = cleanSpaces.Replace(cleanHtml.Replace(body.ToString(), " "), " ").Trim();

                var actions = new IndexAction<Content>[]
                {
                    IndexAction.MergeOrUpload(
                        new Content
                        {
                            Slug = post.Slug,
                            ContentId = post.Id.ToString(),
                            ContentType = "post",
                            Title = post.Title,
                            Category = post.Category.Title,
                            Tags = post.Tags.Select(t => t.Title).ToList(),
                            Body = cleaned
                        }
                    )
                };
                var batch = IndexBatch.New(actions);

                indexClient.Documents.Index(batch);
            }
        }

        public static void PostDelete(PostBase post)
        {

        }

        private static SearchServiceClient CreateClient()
        {
            return new SearchServiceClient(searchServiceName, new SearchCredentials(adminApiKey));
        }
    }
}