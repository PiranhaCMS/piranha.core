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

namespace Piranha.Azure.Search.Services
{
    /// <summary>
    /// The identity module.
    /// </summary>
    public class ContentSearchService
    {
        private readonly string _serviceName = "";
        private readonly string _apiKey = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="serviceName">The search service name</param>
        /// <param name="apiKey">The admin api key</param>
        public ContentSearchService(string serviceName, string apiKey)
        {
            _serviceName = serviceName;
            _apiKey = apiKey;
        }

        public void CreateIndexes()
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

        public void PageSave(PageBase page)
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

        public void PageDelete(PageBase page)
        {

        }

        public void PostSave(PostBase post)
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

        public void PostDelete(PostBase post)
        {

        }

        private SearchServiceClient CreateClient()
        {
            return new SearchServiceClient(_serviceName, new SearchCredentials(_apiKey));
        }
    }
}