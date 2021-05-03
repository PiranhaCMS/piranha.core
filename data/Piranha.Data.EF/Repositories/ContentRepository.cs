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
                return await _service.TransformAsync<T>(content, App.ContentTypes.GetById(content.TypeId), ProcessAsync, languageId)
                    .ConfigureAwait(false);
            }
            return null;
        }

        /// <summary>
        /// Gets all available categories for the specified group.
        /// </summary>
        /// <param name="groupId">The group id</param>
        /// <returns>The available categories</returns>
        public Task<IEnumerable<Models.Taxonomy>> GetAllCategories(string groupId)
        {
            return GetAllTaxonomies(groupId, TaxonomyType.Category);
        }

        /// <summary>
        /// Gets all available tags for the specified groupd.
        /// </summary>
        /// <param name="groupId">The group id</param>
        /// <returns>The available tags</returns>
        public Task<IEnumerable<Models.Taxonomy>> GetAllTags(string groupId)
        {
            return GetAllTaxonomies(groupId, TaxonomyType.Tag);
        }

        /// <summary>
        /// Gets the current translation status for the content model
        /// with the given id.
        /// </summary>
        /// <param name="contentId">The unique content id</param>
        /// <returns>The translation status</returns>
        public async Task<Models.TranslationStatus> GetTranslationStatusById(Guid contentId)
        {
            var allLanguages = await _db.Languages
                .OrderBy(l => l.Title)
                .ToListAsync()
                .ConfigureAwait(false);

            var defaultLang = allLanguages
                .FirstOrDefault(l => l.IsDefault);
            var languages = allLanguages
                .Where(l => !l.IsDefault)
                .ToList();

            var translations = await _db.ContentTranslations
                .Where(t => t.ContentId == contentId)
                .ToListAsync()
                .ConfigureAwait(false);

            return GetTranslationStatus(contentId, defaultLang, languages, translations);
        }

        /// <summary>
        /// Gets the translation summary for the content group with
        /// the given id.
        /// </summary>
        /// <param name="groupId">The group id</param>
        /// <returns>The translation summary</returns>
        public async Task<Models.TranslationSummary> GetTranslationStatusByGroup(string groupId)
        {
            var result = new Models.TranslationSummary
            {
                GroupId = groupId,
            };

            var allLanguages = await _db.Languages
                .OrderBy(l => l.Title)
                .ToListAsync()
                .ConfigureAwait(false);

            var defaultLang = allLanguages
                .FirstOrDefault(l => l.IsDefault);
            var languages = allLanguages
                .Where(l => !l.IsDefault)
                .ToList();

            var translations = await _db.ContentTranslations
                .Where(t => t.Content.Type.Group == groupId)
                .OrderBy(t => t.ContentId)
                .ToListAsync()
                .ConfigureAwait(false);

            var contentIds = translations
                .Select(t => t.ContentId)
                .Distinct()
                .ToList();

            // Get the translation status for each of the
            // content models.
            foreach (var contentId in contentIds)
            {
                var status = GetTranslationStatus(contentId,
                    defaultLang,
                    languages,
                    translations.Where(t => t.ContentId == contentId));

                if (status != null)
                {
                    // Summarize content status
                    status.UpToDateCount = status.Translations.Count(t => t.IsUpToDate);
                    status.TotalCount = status.Translations.Count;
                    status.IsUpToDate = status.UpToDateCount == status.TotalCount;

                    result.Content.Add(status);
                }
            }

            // Summarize
            result.UpToDateCount = result.Content.Sum(c => c.UpToDateCount);
            result.TotalCount = result.Content.Sum(c => c.TotalCount);
            result.IsUpToDate = result.UpToDateCount == result.TotalCount;

            return result;
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
                    .Include(c => c.Blocks).ThenInclude(b => b.Fields).ThenInclude(f => f.Translations)
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

                // Transform blocks
                if (model is Models.IBlockContent blockModel)
                {
                    var blockModels = blockModel.Blocks;

                    if (blockModels != null)
                    {
                        var blocks = _service.TransformContentBlocks(blockModels, languageId);
                        var current = blocks.Select(b => b.Id).ToArray();

                        // Delete removed blocks
                        var removed = content.Blocks
                            .Where(b => !current.Contains(b.Id) && b.ParentId == null)
                            .ToList();
                        var removedItems = content.Blocks
                            .Where(b => !current.Contains(b.Id) && b.ParentId != null) // && removed.Select(p => p.Id).ToList().Contains(b.ParentId.Value))
                            .ToList();

                        _db.ContentBlocks.RemoveRange(removed);
                        _db.ContentBlocks.RemoveRange(removedItems);

                        // Map the new block
                        for (var n = 0; n < blocks.Count; n++)
                        {
                            var block = content.Blocks.FirstOrDefault(b => b.Id == blocks[n].Id);

                            if (block == null)
                            {
                                block = new ContentBlock
                                {
                                    Id = blocks[n].Id != Guid.Empty ? blocks[n].Id : Guid.NewGuid()
                                };
                                await _db.ContentBlocks.AddAsync(block).ConfigureAwait(false);
                            }
                            block.ParentId = blocks[n].ParentId;
                            block.SortOrder = n;
                            block.CLRType = blocks[n].CLRType;

                            var currentFields = blocks[n].Fields.Select(f => f.FieldId).Distinct();
                            var removedFields = block.Fields.Where(f => !currentFields.Contains(f.FieldId));

                            _db.ContentBlockFields.RemoveRange(removedFields);

                            foreach (var newField in blocks[n].Fields)
                            {
                                var field = block.Fields.FirstOrDefault(f => f.FieldId == newField.FieldId);
                                if (field == null)
                                {
                                    field = new ContentBlockField
                                    {
                                        Id = newField.Id != Guid.Empty ? newField.Id : Guid.NewGuid(),
                                        BlockId = block.Id,
                                        FieldId = newField.FieldId
                                    };
                                    await _db.ContentBlockFields.AddAsync(field).ConfigureAwait(false);
                                    block.Fields.Add(field);
                                }
                                field.SortOrder = newField.SortOrder;
                                field.CLRType = newField.CLRType;
                                field.Value = newField.Value;

                                foreach (var newTranslation in newField.Translations)
                                {
                                    var translation = field.Translations.FirstOrDefault(t => t.LanguageId == newTranslation.LanguageId);
                                    if (translation == null)
                                    {
                                        translation = new ContentBlockFieldTranslation
                                        {
                                            FieldId = field.Id,
                                            LanguageId = languageId
                                        };
                                        await _db.ContentBlockFieldTranslations.AddAsync(translation).ConfigureAwait(false);
                                        field.Translations.Add(translation);
                                    }
                                    translation.Value = newTranslation.Value;
                                }
                            }
                            content.Blocks.Add(block);
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

        private async Task<IEnumerable<Models.Taxonomy>> GetAllTaxonomies(string groupId, TaxonomyType type)
        {
            var result = new List<Models.Taxonomy>();
            var taxonomies = await _db.Taxonomies
                .AsNoTracking()
                .Where(t => t.GroupId == groupId && t.Type == type)
                .OrderBy(t => t.Title)
                .ToListAsync()
                .ConfigureAwait(false);

            foreach (var taxonomy in taxonomies)
            {
                result.Add(new Models.Taxonomy
                {
                    Id = taxonomy.Id,
                    Title = taxonomy.Title,
                    Slug = taxonomy.Slug,
                    Type = taxonomy.Type == TaxonomyType.Category ? Models.TaxonomyType.Category : Models.TaxonomyType.Tag
                });
            }
            return result;
        }

        private Models.TranslationStatus GetTranslationStatus(
            Guid contentId,
            Language defaultLanguage,
            IEnumerable<Language> languages,
            IEnumerable<ContentTranslation> translations)
        {
            var defaultTranslation = translations
                .FirstOrDefault(t => t.LanguageId == defaultLanguage.Id);

            if (defaultTranslation != null)
            {
                // Create the result object
                var result = new Models.TranslationStatus
                {
                    ContentId = contentId,
                    Translations = languages
                        .Where(l => !l.IsDefault)
                        .Select(l => new Models.TranslationStatus.TranslationStatusItem
                    {
                        LanguageId = l.Id,
                        LanguageTitle = l.Title
                    }).OrderBy(l => l.LanguageTitle).ToList()
                };

                // Examine the available translations
                foreach (var translation in translations.Where(t => t.LanguageId != defaultLanguage.Id))
                {
                    if (translation.LastModified >= defaultTranslation.LastModified)
                    {
                        result.Translations
                            .FirstOrDefault(t => t.LanguageId == translation.LanguageId)
                            .IsUpToDate = true;
                    }
                }

                // Summarize
                result.TotalCount = result.Translations.Count;
                result.UpToDateCount = result.Translations.Count(t => t.IsUpToDate);
                result.IsUpToDate = result.UpToDateCount == result.TotalCount;

                return result;
            }

            // The content model wasn't available in the default language
            // which means we can't decide if the translations are up to date.
            return null;
        }

        /// <summary>
        /// Gets the base query for content.
        /// </summary>
        /// <returns>The queryable</returns>
        private IQueryable<Content> GetQuery()
        {
            return (IQueryable<Content>)_db.Content
                .AsNoTracking()
                .Include(c => c.Category)
                .Include(c => c.Translations)
                .Include(c => c.Blocks).ThenInclude(b => b.Fields).ThenInclude(f => f.Translations)
                .Include(c => c.Fields).ThenInclude(f => f.Translations)
                .Include(c => c.Tags).ThenInclude(t => t.Taxonomy)
                .AsSplitQuery()
                .AsQueryable();
        }

        /// <summary>
        /// Performs additional processing and loads related models.
        /// </summary>
        /// <param name="content">The source content</param>
        /// <param name="model">The target model</param>
        private Task ProcessAsync<T>(Data.Content content, T model) where T : Models.GenericContent
        {
            return Task.Run(() => {
                // Map category
                if (content.Category != null && model is Models.ICategorizedContent categorizedModel)
                {
                    categorizedModel.Category = new Models.Taxonomy
                    {
                        Id = content.Category.Id,
                        Title = content.Category.Title,
                        Slug = content.Category.Slug
                    };
                }

                // Map tags
                if (model is Models.ITaggedContent taggedContent)
                {
                    foreach (var tag in content.Tags)
                    {
                        taggedContent.Tags.Add(new Models.Taxonomy
                        {
                            Id = tag.Taxonomy.Id,
                            Title = tag.Taxonomy.Title,
                            Slug = tag.Taxonomy.Slug
                        });
                    }
                }

                // Map Blocks
                if (!(model is Models.IContentInfo) && model is Models.IBlockContent blockModel)
                {
                    blockModel.Blocks = _service.TransformBlocks(content.Blocks.OrderBy(b => b.SortOrder), content.SelectedLanguageId);
                }
            });
        }
    }
}
