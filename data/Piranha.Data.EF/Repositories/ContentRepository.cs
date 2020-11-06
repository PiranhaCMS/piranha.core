/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Piranha.Data;
using Piranha.Services;

namespace Piranha.Repositories
{
    public class ContentRepository : IContentRepository
    {
        private readonly IDb _db;
        private readonly IContentService<Content, ContentField, Models.GenericContent> _service;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db connection</param>
        /// <param name="factory">The content service factory</param>
        public ContentRepository(IDb db, IContentServiceFactory factory)
        {
            _db = db;
            _service = factory.CreateContentService();
        }

        /// <summary>
        /// Gets all of the available content for the optional
        /// group id.
        /// </summary>
        /// <param name="groupId">The optional group id</param>
        /// <returns>The available content</returns>
        public async Task<IEnumerable<Guid>> GetAll(string groupId = null)
        {
            var query = _db.Content
                .AsNoTracking();

            if (!string.IsNullOrEmpty(groupId))
            {
                query = query
                    .Where(c => c.Type.Group == groupId);
            }

            return await query
                .OrderBy(c => c.Title)
                .ThenBy(c => c.LastModified)
                .Select(c => c.Id)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the content model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <param name="languageId">The selected language id</param>
        /// <returns>The content model</returns>
        public async Task<T> GetById<T>(Guid id, Guid languageId) where T : Models.GenericContent
        {
            var content = await GetQuery()
                .FirstOrDefaultAsync(c => c.Id == id)
                .ConfigureAwait(false);

            if (content != null)
            {
                return await _service.TransformAsync<T>(content, App.ContentTypes.GetById(content.TypeId), languageId: languageId)
                    .ConfigureAwait(false);
            }
            return null;
        }

        /// <summary>
        /// Saves the given content model
        /// </summary>
        /// <param name="model">The content model</param>
        /// <param name="languageId">The selected language id</param>
        public async Task Save<T>(T model, Guid languageId) where T : Models.GenericContent
        {
            var type = App.ContentTypes.GetById(model.TypeId);
            var lastModified = DateTime.MinValue;

            if (type != null)
            {
                // Ensure category
                if (model is Models.ICategorizedContent categorized)
                {
                    var category = await _db.Taxonomies
                        .FirstOrDefaultAsync(c => c.Id == categorized.Category.Id)
                        .ConfigureAwait(false);

                    if (category == null)
                    {
                        if (!string.IsNullOrWhiteSpace(categorized.Category.Slug))
                        {
                            category = await _db.Taxonomies
                                .FirstOrDefaultAsync(c => c.GroupId == type.Group && c.Slug == categorized.Category.Slug && c.Type == TaxonomyType.Category)
                                .ConfigureAwait(false);
                        }
                        if (category == null && !string.IsNullOrWhiteSpace(categorized.Category.Title))
                        {
                            category = await _db.Taxonomies
                                .FirstOrDefaultAsync(c => c.GroupId == type.Group && c.Title == categorized.Category.Title && c.Type == TaxonomyType.Category)
                                .ConfigureAwait(false);
                        }

                        if (category == null)
                        {
                            category = new Taxonomy
                            {
                                Id = categorized.Category.Id != Guid.Empty ? categorized.Category.Id : Guid.NewGuid(),
                                GroupId = type.Group,
                                Type = TaxonomyType.Category,
                                Title = categorized.Category.Title,
                                Slug = Utils.GenerateSlug(categorized.Category.Title),
                                Created = DateTime.Now,
                                LastModified = DateTime.Now
                            };
                            await _db.Taxonomies.AddAsync(category).ConfigureAwait(false);
                        }
                        categorized.Category.Id = category.Id;
                        categorized.Category.Title = category.Title;
                        categorized.Category.Slug = category.Slug;
                    }
                }

                // Ensure tags
                if (model is Models.ITaggedContent tagged)
                {
                    foreach (var t in tagged.Tags)
                    {
                        var tag = await _db.Taxonomies
                            .FirstOrDefaultAsync(tg => tg.Id == t.Id)
                            .ConfigureAwait(false);

                        if (tag == null)
                        {
                            if (!string.IsNullOrWhiteSpace(t.Slug))
                            {
                                tag = await _db.Taxonomies
                                    .FirstOrDefaultAsync(tg => tg.GroupId == type.Group && tg.Slug == t.Slug && tg.Type == TaxonomyType.Tag)
                                    .ConfigureAwait(false);
                            }
                            if (tag == null && !string.IsNullOrWhiteSpace(t.Title))
                            {
                                tag = await _db.Taxonomies
                                    .FirstOrDefaultAsync(tg => tg.GroupId == type.Group && tg.Title == t.Title && tg.Type == TaxonomyType.Tag)
                                    .ConfigureAwait(false);
                            }

                            if (tag == null)
                            {
                                tag = new Taxonomy
                                {
                                    Id = t.Id != Guid.Empty ? t.Id : Guid.NewGuid(),
                                    GroupId = type.Group,
                                    Type = TaxonomyType.Tag,
                                    Title = t.Title,
                                    Slug = Utils.GenerateSlug(t.Title),
                                    Created = DateTime.Now,
                                    LastModified = DateTime.Now
                                };
                                await _db.Taxonomies.AddAsync(tag).ConfigureAwait(false);
                            }
                            t.Id = tag.Id;
                        }
                        t.Title = tag.Title;
                        t.Slug = tag.Slug;
                    }
                }

                var content = await _db.Content
                    .Include(c => c.Translations)
                    .Include(c => c.Fields).ThenInclude(f => f.Translations)
                    .Include(c => c.Category)
                    .Include(c => c.Tags).ThenInclude(t => t.Taxonomy)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(p => p.Id == model.Id)
                    .ConfigureAwait(false);

                // If not, create new content
                if (content == null)
                {
                    content = new Content
                    {
                        Id = model.Id != Guid.Empty ? model.Id : Guid.NewGuid(),
                        Created = DateTime.Now,
                        LastModified = DateTime.Now
                    };
                    model.Id = content.Id;

                    await _db.Content.AddAsync(content).ConfigureAwait(false);
                }
                else
                {
                    content.LastModified = DateTime.Now;
                }
                content = _service.Transform<T>(model, type, content, languageId);

                // Process fields
                foreach (var field in content.Fields)
                {
                    // Ensure foreign key for new fields
                    if (field.ContentId == Guid.Empty)
                    {
                        field.ContentId = content.Id;
                        await _db.ContentFields.AddAsync(field).ConfigureAwait(false);
                    }
                }

                if (model is Models.ITaggedContent taggedModel)
                {
                    // Remove tags
                    var removedTags = new List<ContentTaxonomy>();
                    foreach (var tag in content.Tags)
                    {
                        if (!taggedModel.Tags.Any(t => t.Id == tag.TaxonomyId))
                        {
                            removedTags.Add(tag);
                        }
                    }
                    foreach (var removed in removedTags)
                    {
                        content.Tags.Remove(removed);
                    }

                    // Add tags
                    foreach (var tag in taggedModel.Tags)
                    {
                        if (!content.Tags.Any(t => t.ContentId == content.Id && t.TaxonomyId == tag.Id))
                        {
                            var contentTaxonomy = new ContentTaxonomy
                            {
                                ContentId = content.Id,
                                TaxonomyId = tag.Id
                            };
                            content.Tags.Add(contentTaxonomy);
                        }
                    }
                }

                await _db.SaveChangesAsync().ConfigureAwait(false);

                /*
                 * TODO
                 *
                await DeleteUnusedCategories(model.BlogId).ConfigureAwait(false);
                await DeleteUnusedTags(model.BlogId).ConfigureAwait(false);
                 */
            }
        }

        /// <summary>
        /// Deletes the content model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task Delete(Guid id)
        {
            var model = await _db.Content
                .Include(c => c.Translations)
                .Include(c => c.Fields).ThenInclude(f => f.Translations)
                .AsSplitQuery()
                .FirstOrDefaultAsync(p => p.Id == id)
                .ConfigureAwait(false);

            if (model != null)
            {
                _db.Content.Remove(model);

                await _db.SaveChangesAsync().ConfigureAwait(false);

                /*
                 * TODO
                 *
                await DeleteUnusedCategories(model.BlogId).ConfigureAwait(false);
                await DeleteUnusedTags(model.BlogId).ConfigureAwait(false);
                 */
            }
        }

        /// <summary>
        /// Gets the base query for content.
        /// </summary>
        /// <returns>The queryable</returns>
        private IQueryable<Content> GetQuery()
        {
            return (IQueryable<Content>)_db.Content
                .AsNoTracking()
                .Include(c => c.Translations)
                .Include(c => c.Fields).ThenInclude(f => f.Translations)
                .AsSplitQuery()
                .AsQueryable();
        }
    }
}
