using Aero.Cms.Data.Data;

using Aero.Cms.Data.Services;
using Aero.Cms.Models;
using Aero.Cms.Repositories;
using Marten;
using Marten.Linq;
using Language = Aero.Cms.Data.Data.Language;
using Taxonomy = Aero.Cms.Data.Data.Taxonomy;
using TaxonomyType = Aero.Cms.Data.Data.TaxonomyType;


namespace Aero.Cms.Data.Repositories;

internal class ContentRepository : IContentRepository
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
    public async Task<IEnumerable<string>> GetAll(string groupId = null)
    {
        var query = _db.Content;

        if (!string.IsNullOrEmpty(groupId))
        {
            query = (IMartenQueryable<Content>)query.Where(c => c.Type.Group == groupId);
        }

        return await query
            .OrderBy(c => c.Title)
            .ThenBy(c => c.LastModified)
            .Select(c => c.Id)
            .ToListAsync();
    }

    /// <summary>
    /// Gets the content model with the specified id.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    /// <param name="id">The unique id</param>
    /// <param name="languageId">The selected language id</param>
    /// <returns>The content model</returns>
    public async Task<T> GetById<T>(string id, string languageId) where T : Models.GenericContent
    {
        var content = await GetQuery()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (content != null)
        {
            return await _service.TransformAsync<T>(content, App.ContentTypes.GetById(content.TypeId), ProcessAsync,
                languageId);
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
    public async Task<Models.TranslationStatus> GetTranslationStatusById(string contentId)
    {
        var allLanguages = await _db.Languages
            
            .OrderBy(l => l.Title)
            .ToListAsync();

        var defaultLang = allLanguages
            .FirstOrDefault(l => l.IsDefault);
        var languages = allLanguages
            .Where(l => !l.IsDefault)
            .ToList();

        // Query the content with embedded translations
        var content = await _db.Content
            
            .Where(c => c.Id == contentId)
            .FirstOrDefaultAsync();

        if (content == null)
        {
            Console.WriteLine($"[DEBUG] Content not found for ID: {contentId}");
        }
        else
        {
            Console.WriteLine($"[DEBUG] Content found: {content.Id}, Title: {content.Title}, Translations: {content.Translations.Count}");
            foreach (var t in content.Translations)
            {
                Console.WriteLine($"[DEBUG] Translation: {t.LanguageId}, Title: {t.Title}");
            }
        }

        Console.WriteLine($"[DEBUG] Languages found: {allLanguages.Count}, Default: {defaultLang?.Id}, Others: {string.Join(", ", languages.Select(l => l.Id))}");

        var translations = content?.Translations ?? new List<ContentTranslation>();
        DateTime lastMod = content?.LastModified ?? DateTime.MinValue;
        
        return this.GetTranslationStatus(contentId, defaultLang, languages, translations, lastMod);
    }

    /// <summary>
    /// Gets the translation summary for the content group with
    /// the given id.
    /// </summary>

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

        var allLanguages = await _db.Languages.OrderBy(l => l.Title)
            .ToListAsync();

        var defaultLang = allLanguages
            .FirstOrDefault(l => l.IsDefault);
        var languages = allLanguages
            .Where(l => !l.IsDefault)
            .ToList();

        // Query content with embedded translations by group
        //var contents = await _db.Content
        //    
        //    .Where(c => c.Type.Group == groupId)
        //    .ToListAsync();

        //var contents = await _db.session.Query<Content_ByTypeGroup.IndexEntry, Content_ByTypeGroup>()

        //    .Where(c => c.Group == groupId)
        //    .OfType<Content>()
        //    .ToListAsync();
        var contents = await _db.session.Query<Content>()
            .Join(
                _db.session.Query<ContentType>(),
                content => content.TypeId,
                type => type.Id,
                (content, type) => new { content, type }
            )
            .Where(x => x.type.Group == groupId)
            .Select(x => x.content) // Select the original Content document
            .ToListAsync();

        Console.WriteLine($"[DEBUG] Group search: {groupId}, Contents found: {contents.Count}");

        // Get the translation status for each of the
        // content models.
        foreach (var content in contents)
        {
            var translations = content?.Translations ?? new List<ContentTranslation>();
            var contentLastModified = content?.LastModified ?? DateTime.MinValue;
            var status = GetTranslationStatus(content.Id,
                defaultLang,
                languages,
                translations,
                contentLastModified);
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
    public async Task Save<T>(T model, string languageId) where T : Models.GenericContent
    {
        var type = App.ContentTypes.GetById(model.TypeId);
        var lastModified = DateTime.MinValue;

        if (type != null)
        {
            // Ensure category
            if (type.UseCategory)
            {
                if (model is Models.ICategorizedContent categorized)
                {
                    var category = await _db.Taxonomies
                        .FirstOrDefaultAsync(c => c.Id == categorized.Category.Id);

                    if (category == null)
                    {
                        if (!string.IsNullOrWhiteSpace(categorized.Category.Slug))
                        {
                            category = await _db.Taxonomies
                                .FirstOrDefaultAsync(c =>
                                    c.GroupId == type.Group && c.Slug == categorized.Category.Slug &&
                                    c.Type == TaxonomyType.Category);
                        }

                        if (category == null && !string.IsNullOrWhiteSpace(categorized.Category.Title))
                        {
                            category = await _db.Taxonomies
                                .FirstOrDefaultAsync(c =>
                                    c.GroupId == type.Group && c.Title == categorized.Category.Title &&
                                    c.Type == TaxonomyType.Category);
                        }

                        if (category == null)
                        {
                            category = new Taxonomy
                            {
                                Id = !string.IsNullOrEmpty(categorized.Category.Id)
                                    ? categorized.Category.Id
                                    : Snowflake.NewId(),
                                GroupId = type.Group,
                                Type = TaxonomyType.Category,
                                Title = categorized.Category.Title,
                                Slug = Utils.GenerateSlug(categorized.Category.Title),
                                Created = DateTime.Now,
                                LastModified = DateTime.Now
                            };
                            //await _db.Taxonomies.AddAsync(category).ConfigureAwait(false);
                            _db.session.Store(category);
                        }

                        categorized.Category.Id = category.Id;
                        categorized.Category.Title = category.Title;
                        categorized.Category.Slug = category.Slug;
                    }
                }
            }

            // Ensure tags
            if (type.UseTags)
            {
                if (model is Models.ITaggedContent tagged)
                {
                    foreach (var t in tagged.Tags)
                    {
                        var tag = await _db.Taxonomies
                            .FirstOrDefaultAsync(tg => tg.Id == t.Id);

                        if (tag == null)
                        {
                            if (!string.IsNullOrWhiteSpace(t.Slug))
                            {
                                tag = await _db.Taxonomies
                                    .FirstOrDefaultAsync(tg =>
                                        tg.GroupId == type.Group && tg.Slug == t.Slug && tg.Type == TaxonomyType.Tag);
                            }

                            if (tag == null && !string.IsNullOrWhiteSpace(t.Title))
                            {
                                tag = await _db.Taxonomies
                                    .FirstOrDefaultAsync(tg =>
                                        tg.GroupId == type.Group && tg.Title == t.Title && tg.Type == TaxonomyType.Tag);
                            }

                            if (tag == null)
                            {
                                tag = new Taxonomy
                                {
                                    Id = !string.IsNullOrEmpty(t.Id) ? t.Id : Snowflake.NewId(),
                                    GroupId = type.Group,
                                    Type = TaxonomyType.Tag,
                                    Title = t.Title,
                                    Slug = Utils.GenerateSlug(t.Title),
                                    Created = DateTime.Now,
                                    LastModified = DateTime.Now
                                };
                                //await _db.Taxonomies.AddAsync(tag).ConfigureAwait(false);
                                _db.session.Store(tag);
                            }

                            t.Id = tag.Id;
                        }

                        t.Title = tag.Title;
                        t.Slug = tag.Slug;
                    }
                }
            }

            var content = await _db.Content
                .FirstOrDefaultAsync(p => p.Id == model.Id);

            // If not, create new content
            if (content == null)
            {
                content = new Content
                {
                    Id = !string.IsNullOrEmpty(model.Id) ? model.Id : Snowflake.NewId(),
                    Created = DateTime.Now,
                    LastModified = DateTime.Now
                };
                model.Id = content.Id;

                //await _db.Content.AddAsync(content).ConfigureAwait(false);
                _db.session.Store(content);
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
                if (string.IsNullOrEmpty(field.ContentId))
                {
                    field.ContentId = content.Id;
                    //await _db.ContentFields.AddAsync(field).ConfigureAwait(false);
                    _db.session.Store(field);
                }
            }

            if (type.UseTags)
            {
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
                        .Where(b => !current.Contains(b.Id) &&
                                    b.ParentId !=
                                    null) // && removed.Select(p => p.Id).ToList().Contains(b.ParentId.Value))
                        .ToList();

                    // _db.ContentBlocks.RemoveRange(removed);
                    // _db.ContentBlocks.RemoveRange(removedItems);
                    foreach (var item in removed)
                        _db.session.Delete(item);

                    foreach (var item in removedItems)
                        _db.session.Delete(item);


                    // Map the new block
                    for (var n = 0; n < blocks.Count; n++)
                    {
                        var block = content.Blocks.FirstOrDefault(b => b.Id == blocks[n].Id);

                        if (block == null)
                        {
                            block = new ContentBlock
                            {
                                Id = !string.IsNullOrEmpty(blocks[n].Id) ? blocks[n].Id : Snowflake.NewId()
                            };
                            //await _db.ContentBlocks.AddAsync(block).ConfigureAwait(false);
                            _db.session.Store(block);
                        }

                        block.ParentId = blocks[n].ParentId;
                        block.SortOrder = n;
                        block.CLRType = blocks[n].CLRType;

                        var currentFields = blocks[n].Fields.Select(f => f.FieldId).Distinct();
                        var removedFields = block.Fields.Where(f => !currentFields.Contains(f.FieldId));

                        //_db.ContentBlockFields.RemoveRange(removedFields);
                        foreach (var item in removedFields)
                            _db.session.Delete(item);


                        foreach (var newField in blocks[n].Fields)
                        {
                            var field = block.Fields.FirstOrDefault(f => f.FieldId == newField.FieldId);
                            if (field == null)
                            {
                                field = new ContentBlockField
                                {
                                    Id = !string.IsNullOrEmpty(newField.Id) ? newField.Id : Snowflake.NewId(),
                                    BlockId = block.Id,
                                    FieldId = newField.FieldId
                                };
                                //await _db.ContentBlockFields.AddAsync(field).ConfigureAwait(false);
                                _db.session.Store(field);
                                block.Fields.Add(field);
                            }

                            field.SortOrder = newField.SortOrder;
                            field.CLRType = newField.CLRType;
                            field.Value = newField.Value;

                            foreach (var newTranslation in newField.Translations)
                            {
                                var translation =
                                    field.Translations.FirstOrDefault(t => t.LanguageId == newTranslation.LanguageId);
                                if (translation == null)
                                {
                                    translation = new ContentBlockFieldTranslation
                                    {
                                        FieldId = field.Id,
                                        LanguageId = languageId
                                    };
                                    //await _db.ContentBlockFieldTranslations.AddAsync(translation).ConfigureAwait(false);
                                    _db.session.Store(translation);
                                    field.Translations.Add(translation);
                                }

                                translation.Value = newTranslation.Value;
                            }
                        }

                        content.Blocks.Add(block);
                    }
                }
            }

            await _db.SaveChangesAsync();

            /*
                * TODO
                *
            await DeleteUnusedCategories(model.BlogId);
            await DeleteUnusedTags(model.BlogId);
                */
        }
    }

    /// <summary>
    /// Deletes the content model with the specified id.
    /// </summary>
    /// <param name="id">The unique id</param>
    public async Task Delete(string id)
    {
        var model = await _db.Content
            .FirstOrDefaultAsync(p => p.Id == id);

        if (model != null)
        {
            //_db.Content.Remove(model);
            _db.session.Delete(model);

            await _db.SaveChangesAsync();

            /*
                * TODO
                *
            await DeleteUnusedCategories(model.BlogId);
            await DeleteUnusedTags(model.BlogId);
                */
        }
    }

    private async Task<IEnumerable<Models.Taxonomy>> GetAllTaxonomies(string groupId, TaxonomyType type)
    {
        var result = new List<Models.Taxonomy>();
        var taxonomies = await _db.Taxonomies
            .Where(t => t.GroupId == groupId && t.Type == type)
            .OrderBy(t => t.Title)
            .ToListAsync();

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
        string contentId,
        Language defaultLanguage,
        IEnumerable<Language> languages,
        IEnumerable<ContentTranslation> translations,
        DateTime contentLastModified)
    {
        // In the embedded model, the default language content is stored in the Content itself.
        // Translations collection only contains non-default language translations.
        // Use contentLastModified as the reference for the default language.

        // Create the result object
        var result = new Models.TranslationStatus
        {
            ContentId = contentId,
            Translations = languages
                .Where(l => !l.IsDefault)
                .Select(l => new Models.TranslationStatus.TranslationStatusItem
                {
                    LanguageId = l.Id,
                    LanguageTitle = l.Title,
                    IsUpToDate = false  // Explicitly default to not up-to-date
                }).OrderBy(l => l.LanguageTitle).ToList()
        };

        // Create a lookup of actual translations for O(1) access
        var translationLookup = translations.ToDictionary(t => t.LanguageId, t => t);

        // Examine each language that should have a translation
        foreach (var statusItem in result.Translations)
        {
            if (translationLookup.TryGetValue(statusItem.LanguageId, out var translation))
            {
                // Translation exists - mark as up to date
                // In the denormalized model, if translation exists in the document, it's up to date
                statusItem.IsUpToDate = true;
            }
            // If translation doesn't exist, IsUpToDate remains false
        }
        
        // Summarize
        result.TotalCount = result.Translations.Count;
        result.UpToDateCount = result.Translations.Count(t => t.IsUpToDate);
        result.IsUpToDate = result.UpToDateCount == result.TotalCount;

        return result;
    }

    /// <summary>
    /// Gets the base query for content.
    /// </summary>
    /// <returns>The queryable</returns>
    private IMartenQueryable<Content> GetQuery()
    {
        return _db.Content;
    }

    /// <summary>
    /// Performs additional processing and loads related models.
    /// </summary>
    /// <param name="content">The source content</param>
    /// <param name="model">The target model</param>
    private Task ProcessAsync<T>(Content content, T model) where T : Models.GenericContent
    {
        return Task.Run(() =>
        {
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
                blockModel.Blocks = _service.TransformBlocks(content.Blocks.OrderBy(b => b.SortOrder),
                    content.SelectedLanguageId);
            }
        });
    }
}

