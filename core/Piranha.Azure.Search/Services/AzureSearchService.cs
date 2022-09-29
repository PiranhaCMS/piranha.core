/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Text;
using System.Text.RegularExpressions;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Piranha.Extend;
using Piranha.Models;

namespace Piranha.Azure.Search.Services;

/// <summary>
/// The identity module.
/// </summary>
public class AzureSearchService : ISearch
{
    private readonly string _serviceUrl = "";
    private readonly string _apiKey = "";
    private readonly string _index = "";

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="serviceUrl">The search service url</param>
    /// <param name="apiKey">The admin api key</param>
    /// <param name="index">Name of the search index</param>
    public AzureSearchService(string serviceUrl, string apiKey, string index)
    {
        _serviceUrl = serviceUrl;
        _apiKey = apiKey;
        _index = index.ToLowerInvariant();

        // Make sure the search indexes are up to date
        CreateIndex(_index);
    }

    /// <summary>
    /// Creates the main search indexes.
    /// </summary>
    private void CreateIndex(string indexName)
    {
        var indexClient = CreateSearchIndexClient();
        FieldBuilder fieldBuilder = new FieldBuilder();
        var searchFields = fieldBuilder.Build(typeof(Content));

        var definition = new SearchIndex(indexName, searchFields);

        indexClient.CreateOrUpdateIndex(definition);
    }

    /// <summary>
    /// Creates or updates the searchable content for the
    /// given page.
    /// </summary>
    /// <param name="page">The page</param>
    public async Task SavePageAsync(PageBase page)
    {
        var client = CreateSearchClient();

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

        var cleaned = cleanSpaces.Replace(cleanHtml.Replace(body.ToString(), " "), " ").Trim();

        IndexDocumentsBatch<Content> batch = IndexDocumentsBatch.Create(
            IndexDocumentsAction.MergeOrUpload(
                            new Content
                            {
                                Slug = page.Slug,
                                ContentId = page.Id.ToString(),
                                ContentType = "page",
                                Title = page.Title,
                                Body = cleaned
                            }
            )
        );
        await client.IndexDocumentsAsync(batch);
    }

    /// <summary>
    /// Deletes the given page from the search index.
    /// </summary>
    /// <param name="page">The page to delete</param>
    public async Task DeletePageAsync(PageBase page)
    {
        var client = CreateSearchClient();
        var batch = IndexDocumentsBatch.Delete("contentId", new List<string> { page.Id.ToString() });
        await client.IndexDocumentsAsync(batch);
    }

    /// <summary>
    /// Creates or updates the searchable content for the
    /// given post.
    /// </summary>
    /// <param name="post">The post</param>
    public async Task SavePostAsync(PostBase post)
    {
        var client = CreateSearchClient();
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

        var cleaned = cleanSpaces.Replace(cleanHtml.Replace(body.ToString(), " "), " ").Trim();

        IndexDocumentsBatch<Content> batch = IndexDocumentsBatch.Create(
            IndexDocumentsAction.MergeOrUpload(
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
        );
        await client.IndexDocumentsAsync(batch);
    }

    /// <summary>
    /// Deletes the given post from the search index.
    /// </summary>
    /// <param name="post">The post to delete</param>
    public async Task DeletePostAsync(PostBase post)
    {
        var client = CreateSearchClient();
        var batch = IndexDocumentsBatch.Delete("contentId", new List<string> { post.Id.ToString() });
        await client.IndexDocumentsAsync(batch);
    }

    /// <summary>
    /// Creates the SearchIndexClient.
    /// </summary>
    private SearchIndexClient CreateSearchIndexClient()
    {
        SearchIndexClient indexClient = new SearchIndexClient(new Uri(_serviceUrl), new AzureKeyCredential(_apiKey));
        return indexClient;
    }

    /// <summary>
    /// Creates the SearchClient.
    /// </summary>
    private SearchClient CreateSearchClient()
    {
        SearchClient searchClient = new SearchClient(new Uri(_serviceUrl), _index, new AzureKeyCredential(_apiKey));
        return searchClient;
    }
}
