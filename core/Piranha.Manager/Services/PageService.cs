/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Security.Cryptography;
using System.Text;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Manager.Extensions;
using Piranha.Models;
using Piranha.Manager.Models;
using Piranha.Manager.Models.Content;
using Piranha.Services;

namespace Piranha.Manager.Services;

public class PageService
{
    private readonly IApi _api;
    private readonly IContentFactory _factory;
    private readonly ManagerLocalizer _localizer;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="api">The current api</param>
    /// <param name="factory">The content factory</param>
    /// <param name="localizer">The manager localizer</param>
    public PageService(IApi api, IContentFactory factory, ManagerLocalizer localizer)
    {
        _api = api;
        _factory = factory;
        _localizer = localizer;
    }

    /// <summary>
    /// Gets the list model.
    /// </summary>
    /// <returns>The list model</returns>
    public async Task<PageListModel> GetList()
    {
        var model = new PageListModel
        {
            Sites = (await _api.Sites.GetAllAsync())
                .OrderByDescending(s => s.IsDefault)
                .Select(s => new PageListModel.PageSite
            {
                Id = s.Id,
                Title = s.Title,
                Slug = "/",
                EditUrl = "manager/site/edit/"
            }).ToList(),
            PageTypes = App.PageTypes.Select(t => new ContentTypeModel
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                AddUrl = "manager/page/add/"
            }).ToList()
        };

        foreach (var site in model.Sites)
        {
            site.Pages.AddRange(await GetPageStructure(site.Id));
        }
        return model;
    }

    /// <summary>
    /// Gets the hierarchical page structure for the specified site.
    /// </summary>
    /// <param name="siteId">The site id</param>
    /// <returns>The structure</returns>
    private async Task<List<PageListModel.PageItem>> GetPageStructure(Guid siteId)
    {
        var pages = new List<PageListModel.PageItem>();

        // Get the configured expanded levels
        var expandedLevels = 0;
        using (var config = new Config(_api))
        {
            expandedLevels = config.ManagerExpandedSitemapLevels;
        }

        // Get the sitemap and transform
        var sitemap = await _api.Sites.GetSitemapAsync(siteId, false);
        var drafts = await _api.Pages.GetAllDraftsAsync(siteId);
        foreach (var item in sitemap)
        {
            pages.Add(MapRecursive(siteId, item, 0, expandedLevels, drafts));
        }
        return pages;
    }

    private async Task<List<PageListModel.PageItem>> GetArchivePages(Guid siteId)
    {
        var archives = new List<PageListModel.PageItem>();

        // Get all archive page types
        var archiveTypes = (await _api.PageTypes.GetAllAsync())
            .Where(t => t.IsArchive).Select(t => t.Id);
        var drafts = await _api.Pages.GetAllDraftsAsync(siteId);

        // Get all pages for the site
        return (await _api.Pages.GetAllAsync(siteId))
            .Where(p => archiveTypes.Contains(p.TypeId))
            .OrderBy(p => p.Title)
            .Select(p => new PageListModel.PageItem
            {
                Id = p.Id,
                Permalink = p.Permalink,
                Status = drafts.Contains(p.Id) ? _localizer.General[PageListModel.PageItem.Draft] :
                    !p.Published.HasValue ? _localizer.General[PageListModel.PageItem.Unpublished] : "",
                Title = p.Title
            }).ToList();
    }

    /// <summary>
    /// Gets the site list with the page structure of the selected site for
    /// the page picker.
    /// </summary>
    /// <param name="siteId">The current site</param>
    /// <returns>The model</returns>
    public async Task<SiteListModel> GetSiteList(Guid siteId)
    {
        var site = await _api.Sites.GetByIdAsync(siteId);

        var model = new SiteListModel
        {
            SiteId = siteId,
            SiteTitle = site.Title,
            Sites = (await _api.Sites.GetAllAsync())
                .OrderByDescending(s => s.IsDefault)
                .Select(s => new PageListModel.PageSite
            {
                Id = s.Id,
                Title = s.Title,
                Slug = "/",
                EditUrl = "manager/site/edit/"
            }).ToList(),
            Items = await GetPageStructure(siteId)
        };
        return model;
    }

    /// <summary>
    /// Gets the list of archive pages for the selected site for
    /// the archive picker.
    /// </summary>
    /// <param name="siteId">The current site</param>
    /// <returns>The model</returns>
    public async Task<SiteListModel> GetArchiveList(Guid siteId)
    {
        var site = await _api.Sites.GetByIdAsync(siteId);

        var model = new SiteListModel
        {
            SiteId = siteId,
            SiteTitle = site.Title,
            Sites = (await _api.Sites.GetAllAsync())
                .OrderByDescending(s => s.IsDefault)
                .Select(s => new PageListModel.PageSite
            {
                Id = s.Id,
                Title = s.Title,
                Slug = "/",
                EditUrl = "manager/site/edit/"
            }).ToList(),
            Items = await GetArchivePages(siteId)
        };
        return model;
    }

    /// <summary>
    /// Gets the sitemap model.
    /// </summary>
    /// <returns>The list model</returns>
    public async Task<Sitemap> GetSitemap(Guid? siteId = null)
    {
        return await _api.Sites.GetSitemapAsync(siteId, false);
    }

    public async Task<PageEditModel> Create(Guid siteId, string typeId)
    {
        var page = await _api.Pages.CreateAsync<DynamicPage>(typeId);

        if (page != null)
        {
            page.Id = Guid.NewGuid();
            page.SiteId = siteId;
            page.SortOrder = (await _api.Sites.GetSitemapAsync(page.SiteId, false)).Count;

            // Perform manager init
            await _factory.InitDynamicManagerAsync(page,
                App.PageTypes.GetById(page.TypeId));

            return Transform(page, false);
        }
        return null;
    }

    public async Task<PageEditModel> CreateRelative(Guid pageId, string typeId, bool after)
    {
        var relative = await _api.Pages.GetByIdAsync<PageInfo>(pageId);

        if (relative != null)
        {
            var page = await _api.Pages.CreateAsync<DynamicPage>(typeId);

            page.Id = Guid.NewGuid();
            page.SiteId = relative.SiteId;
            page.ParentId = after ? relative.ParentId : relative.Id;
            page.SortOrder = after ? relative.SortOrder + 1 : 0;

            if (page != null)
            {
                // Perform manager init
                await _factory.InitDynamicManagerAsync(page,
                    App.PageTypes.GetById(page.TypeId));

                return Transform(page, false);
            }
        }
        return null;
    }

    public async Task<PageEditModel> Copy(Guid sourceId, Guid siteId)
    {
        var original = await _api.Pages.GetByIdAsync(sourceId);

        if (original != null)
        {
            var page = await _api.Pages.CopyAsync(original);

            page.SiteId = siteId;
            page.SortOrder = (await _api.Sites.GetSitemapAsync(page.SiteId, false)).Count;
            if (original.SiteId != siteId)
            {
                page.ParentId = null;
            }

            // Perform manager init
            await _factory.InitDynamicManagerAsync(page,
                App.PageTypes.GetById(page.TypeId));

            return Transform(page, false);
        }
        return null;
    }

    public async Task<PageEditModel> CopyRelative(Guid sourceId, Guid pageId, bool after)
    {
        var relative = await _api.Pages.GetByIdAsync<PageInfo>(pageId);

        if (relative != null)
        {
            var original = await _api.Pages.GetByIdAsync(sourceId);

            if (original != null)
            {
                var page = await _api.Pages.CopyAsync(original);

                page.SiteId = relative.SiteId;
                page.ParentId = after ? relative.ParentId : relative.Id;
                page.SortOrder = after ? relative.SortOrder + 1 : 0;

                // Perform manager init
                await _factory.InitDynamicManagerAsync(page,
                    App.PageTypes.GetById(page.TypeId));

                return Transform(page, false);
            }
        }
        return null;
    }

    public async Task<PageEditModel> GetById(Guid id, bool useDraft = true, Guid? languageId = null)
    {
        var languages = await _api.Languages.GetAllAsync();
        var defaultLanguageId = languages.FirstOrDefault(l => l.IsDefault)?.Id;
        var contentLanguageId = languageId == defaultLanguageId ? null : languageId;
        var isDraft = true;
        var useDefaultLanguageDraft = useDraft &&
            !contentLanguageId.HasValue;
        var page = useDefaultLanguageDraft ? await _api.Pages.GetDraftByIdAsync(id) : null;

        if (page == null)
        {
            page = await _api.Pages.GetByIdAsync(id, contentLanguageId);
            isDraft = false;
        }

        if (page != null)
        {
            if (contentLanguageId.HasValue && useDraft)
            {
                var defaultDraft = await _api.Pages.GetDraftByIdAsync(id);
                if (defaultDraft != null)
                {
                    MergeDraftBlockStructure(page, defaultDraft);
                }
            }

            // Perform manager init
            await _factory.InitDynamicManagerAsync(page,
                App.PageTypes.GetById(page.TypeId));

            var model = Transform(page, isDraft);

            model.PendingCommentCount = (await _api.Pages.GetAllPendingCommentsAsync(id))
                .Count();

            // Populate language data
            model.Languages = languages;
            model.UseTranslations = languages.Count() > 1;
            model.LanguageId = languageId ?? defaultLanguageId;

            return model;
        }
        return null;
    }

    /// <summary>
    /// Gets a page edit model for every configured language.
    /// </summary>
    /// <param name="id">The page id</param>
    /// <param name="useDraft">If draft content should be loaded</param>
    /// <returns>The language-specific page edit models.</returns>
    public async Task<PageTranslationEditModel> GetTranslationsById(Guid id, bool useDraft = true)
    {
        var result = new PageTranslationEditModel();
        var languages = (await _api.Languages.GetAllAsync())
            .OrderByDescending(language => language.IsDefault);

        foreach (var language in languages)
        {
            var page = await GetById(id, useDraft, language.Id);

            if (page != null)
            {
                result.Pages.Add(page);
            }
        }
        return result;
    }

    /// <summary>
    /// Exports the text values of a page for one source and target language pair.
    /// </summary>
    public async Task<PageTranslationExchangeModel> ExportTranslations(Guid id, Guid sourceLanguageId, Guid targetLanguageId)
    {
        var languages = (await _api.Languages.GetAllAsync()).ToList();
        ValidateTranslationLanguages(languages, sourceLanguageId, targetLanguageId);

        var source = await GetById(id, true, sourceLanguageId);
        var target = await GetById(id, true, targetLanguageId);
        var defaultLanguage = languages.First(language => language.IsDefault);
        var structure = await GetById(id, true, defaultLanguage.Id);

        if (source == null || target == null || structure == null)
        {
            throw new ValidationException("The requested page could not be found.");
        }

        return new PageTranslationExchangeModel
        {
            PageId = source.Id,
            SiteId = source.SiteId,
            SourceLanguage = MapExchangeLanguage(languages.Single(language => language.Id == sourceLanguageId)),
            TargetLanguage = MapExchangeLanguage(languages.Single(language => language.Id == targetLanguageId)),
            ExportedAt = DateTime.UtcNow,
            StructureHash = GetTranslationStructureHash(structure),
            Units = CreateTranslationUnits(source, target)
        };
    }

    /// <summary>
    /// Imports supplied target-language text values into an existing page translation.
    /// </summary>
    public async Task<PageTranslationExchangeImportResult> ImportTranslations(Guid id, PageTranslationExchangeModel document)
    {
        if (document == null || document.FormatVersion != "1.0")
        {
            throw new ValidationException("The translation file format is not supported.");
        }
        if (document.PageId != id)
        {
            throw new ValidationException("The translation file belongs to a different page.");
        }
        if (document.SourceLanguage == null || document.TargetLanguage == null)
        {
            throw new ValidationException("The translation file must specify source and target languages.");
        }

        var languages = (await _api.Languages.GetAllAsync()).ToList();
        ValidateTranslationLanguages(languages, document.SourceLanguage.Id, document.TargetLanguage.Id);

        var defaultLanguage = languages.First(language => language.IsDefault);
        var structure = await GetById(id, true, defaultLanguage.Id);
        var target = await GetById(id, true, document.TargetLanguage.Id);

        if (structure == null || target == null || structure.SiteId != document.SiteId)
        {
            throw new ValidationException("The translation file does not match the current page.");
        }
        if (!string.Equals(document.StructureHash, GetTranslationStructureHash(structure), StringComparison.Ordinal))
        {
            throw new ValidationException("The page structure has changed since this translation file was exported. Export a new file before importing.");
        }
        if (document.Units == null || document.Units.Any(unit => unit == null || string.IsNullOrWhiteSpace(unit.Key)) ||
            document.Units.GroupBy(unit => unit.Key).Any(group => group.Count() > 1))
        {
            throw new ValidationException("The translation file contains duplicate or invalid text units.");
        }

        var writable = GetWritableTranslationUnits(target);
        var result = new PageTranslationExchangeImportResult
        {
            TargetLanguageId = document.TargetLanguage.Id
        };

        foreach (var unit in document.Units)
        {
            if (!writable.TryGetValue(unit.Key, out var destination) ||
                !string.Equals(unit.FieldType, destination.FieldType, StringComparison.Ordinal))
            {
                throw new ValidationException($"The translation unit '{unit.Key}' does not match the current page structure.");
            }

            if (unit.Target != null)
            {
                destination.SetValue(unit.Target);
                result.Replaced++;
                if (unit.Target.Length == 0)
                {
                    result.Cleared++;
                }
            }
        }

        await Save(target, true);
        return result;
    }

    public async Task<PageEditModel> Detach(Guid id)
    {
        var page = await _api.Pages.GetByIdAsync(id);

        if (page != null)
        {
            await _api.Pages.DetachAsync(page);

            page = await _api.Pages.GetByIdAsync(id);

            // Perform manager init
            await _factory.InitDynamicManagerAsync(page,
                App.PageTypes.GetById(page.TypeId));

            return Transform(page, false);
        }
        return null;
    }

    public async Task Save(PageEditModel model, bool draft)
    {
        var pageType = App.PageTypes.GetById(model.TypeId);

        if (pageType != null)
        {
            if (model.Id == Guid.Empty)
            {
                model.Id = Guid.NewGuid();
            }

            var page = await _api.Pages.GetByIdAsync(model.Id);

            if (page == null)
            {
                page = await _factory.CreateAsync<DynamicPage>(pageType);
                page.Id = model.Id;
            }

            var defaultLanguage = await _api.Languages.GetDefaultAsync();
            var languageId = model.LanguageId.HasValue && defaultLanguage != null && model.LanguageId.Value != defaultLanguage.Id
                ? model.LanguageId
                : null;

            page.SiteId = model.SiteId;
            page.ParentId = model.ParentId;
            page.OriginalPageId = model.OriginalId;
            page.SortOrder = model.SortOrder;
            page.TypeId = model.TypeId;
            page.Title = model.Title;
            page.NavigationTitle = model.NavigationTitle;
            page.Slug = model.Slug;
            page.MetaTitle = model.MetaTitle;
            page.MetaKeywords = model.MetaKeywords;
            page.MetaDescription = model.MetaDescription;
            page.MetaIndex = model.MetaIndex;
            page.MetaFollow = model.MetaFollow;
            page.MetaPriority = model.MetaPriority;
            page.OgTitle = model.OgTitle;
            page.OgDescription = model.OgDescription;
            page.OgImage = model.OgImage;
            page.PrimaryImage = model.PrimaryImage;
            page.Excerpt = model.Excerpt;
            page.IsHidden = model.IsHidden;
            page.Published = ParsePublishedDate(model); // !string.IsNullOrEmpty(model.Published) ? DateTime.Parse(model.Published) : (DateTime?)null;
            page.RedirectUrl = model.RedirectUrl;
            page.RedirectType = (RedirectType)Enum.Parse(typeof(RedirectType), model.RedirectType);
            page.EnableComments = model.EnableComments;
            page.CloseCommentsAfterDays = model.CloseCommentsAfterDays;
            page.Permissions = model.SelectedPermissions;

            if (pageType.Routes.Count > 1)
            {
                page.Route = pageType.Routes.FirstOrDefault(r => r.Route == model.SelectedRoute?.Route)
                                ?? pageType.Routes.First();
            }

            //
            // Make sure we only keep permissions for pages are registered
            //
            var currentPermissions = App.Permissions.GetPublicPermissions().Select(p => p.Name);
            page.Permissions = page.Permissions.Where(p => currentPermissions.Contains(p)).ToList();

            //
            // We only need to save regions & blocks for pages that are not copies
            //
            if (!page.OriginalPageId.HasValue)
            {
                if (languageId.HasValue)
                {
                    var structurePage = page;
                    var defaultDraft = await _api.Pages.GetDraftByIdAsync(model.Id);
                    if (defaultDraft != null)
                    {
                        structurePage = defaultDraft;
                    }
                    ValidateTranslatedBlockStructure(structurePage.Blocks, model.Blocks);
                    ValidateTranslatedBlockLabels(structurePage.Blocks, model.Blocks);
                }

                // Save regions
                foreach (var region in pageType.Regions)
                {
                    var modelRegion = model.Regions
                        .FirstOrDefault(r => r.Meta.Id == region.Id);

                    if (region.Collection)
                    {
                        var listRegion = (IRegionList)((IDictionary<string, object>)page.Regions)[region.Id];

                        listRegion.Clear();

                        foreach (var item in modelRegion.Items)
                        {
                            if (region.Fields.Count == 1)
                            {
                                listRegion.Add(item.Fields[0].Model);
                            }
                            else
                            {
                                var pageRegion = new ExpandoObject();

                                foreach (var field in region.Fields)
                                {
                                    var modelField = item.Fields
                                        .FirstOrDefault(f => f.Meta.Id == field.Id);
                                    ((IDictionary<string, object>)pageRegion)[field.Id] = modelField.Model;
                                }
                                listRegion.Add(pageRegion);
                            }
                        }
                    }
                    else
                    {
                        var pageRegion = ((IDictionary<string, object>)page.Regions)[region.Id];

                        if (region.Fields.Count == 1)
                        {
                            ((IDictionary<string, object>)page.Regions)[region.Id] =
                                modelRegion.Items[0].Fields[0].Model;
                        }
                        else
                        {
                            foreach (var field in region.Fields)
                            {
                                var modelField = modelRegion.Items[0].Fields
                                    .FirstOrDefault(f => f.Meta.Id == field.Id);
                                ((IDictionary<string, object>)pageRegion)[field.Id] = modelField.Model;
                            }
                        }
                    }
                }

                // Save blocks
                page.Blocks.Clear();

                foreach (var block in model.Blocks)
                {
                    if (block is BlockGroupModel blockGroup)
                    {
                        var groupType = App.Blocks.GetByType(blockGroup.Type);

                        if (groupType != null)
                        {
                            var pageBlock = (BlockGroup)Activator.CreateInstance(groupType.Type);

                            pageBlock.Id = blockGroup.Id;
                            pageBlock.Type = blockGroup.Type;
                            pageBlock.Label = NormalizeBlockLabel(blockGroup.Label);

                            foreach (var field in blockGroup.Fields)
                            {
                                var prop = pageBlock.GetType().GetProperty(field.Meta.Id, App.PropertyBindings);
                                prop.SetValue(pageBlock, field.Model);
                            }

                            foreach (var item in blockGroup.Items)
                            {
                                if (item is BlockItemModel blockItem)
                                {
                                    blockItem.Model.Label = NormalizeBlockLabel(blockItem.Model.Label);
                                    pageBlock.Items.Add(blockItem.Model);
                                }
                                else if (item is BlockGenericModel blockGeneric)
                                {
                                    var transformed = ContentUtils.TransformGenericBlock(blockGeneric);

                                    if (transformed != null)
                                    {
                                        transformed.Label = NormalizeBlockLabel(blockGeneric.Label);
                                        pageBlock.Items.Add(transformed);
                                    }
                                }
                            }
                            page.Blocks.Add(pageBlock);
                        }
                    }
                    else if (block is BlockItemModel blockItem)
                    {
                        blockItem.Model.Label = NormalizeBlockLabel(blockItem.Model.Label);
                        page.Blocks.Add(blockItem.Model);
                    }
                    else if (block is BlockGenericModel blockGeneric)
                    {
                        var transformed = ContentUtils.TransformGenericBlock(blockGeneric);

                        if (transformed != null)
                        {
                            transformed.Label = NormalizeBlockLabel(blockGeneric.Label);
                            page.Blocks.Add(transformed);
                        }
                    }
                }
            }

            // Only pass languageId for non-default languages
            // Save page
            if (draft)
            {
                await _api.Pages.SaveDraftAsync(page, languageId);
            }
            else
            {
                await _api.Pages.SaveAsync(page, languageId);
            }
        }
        else
        {
            throw new ValidationException("Invalid Page Type.");
        }
    }

    /// <summary>
    /// Deletes the page with the given id.
    /// </summary>
    /// <param name="id">The unique id</param>
    public Task Delete(Guid id)
    {
        return _api.Pages.DeleteAsync(id);
    }

    /// <summary>
    /// Updates the sitemap according to the given structure. Please note
    /// that only the first page that has changed position is moved.
    /// </summary>
    /// <param name="structure">The page structure</param>
    public async Task<bool> MovePages(StructureModel structure)
    {
        var pos = GetPosition(structure.Id, structure.Items);

        if (pos != null)
        {
            var page = await _api.Pages.GetByIdAsync<PageInfo>(structure.Id);

            if (page != null)
            {
                await _api.Pages.MoveAsync(page, pos.Item1, pos.Item2);

                return true;
            }
        }
        return false;
    }

    private Tuple<Guid?,int> GetPosition(Guid id, IList<StructureModel.StructureItem> items, Guid? parentId = null)
    {
        for (var n = 0; n < items.Count; n++)
        {
            if (id == new Guid(items[n].Id))
            {
                return new Tuple<Guid?, int>(parentId, n);
            }
            else if (items[n].Children.Count > 0)
            {
                var pos = GetPosition(id, items[n].Children, new Guid(items[n].Id));

                if (pos != null)
                {
                    return pos;
                }
            }
        }
        return null;
    }

    private static void ValidateTranslatedBlockStructure(IList<Block> blocks, IList<BlockModel> submittedBlocks)
    {
        var current = new List<BlockStructure>();
        foreach (var block in blocks)
        {
            AddBlockStructure(current, block, null);
        }

        var submitted = new List<BlockStructure>();
        foreach (var block in submittedBlocks ?? new List<BlockModel>())
        {
            AddBlockStructure(submitted, block, null);
        }

        if (current.Count != submitted.Count || current.Where((block, index) =>
            block.Id != submitted[index].Id ||
            block.ParentId != submitted[index].ParentId ||
            !string.Equals(block.Type, submitted[index].Type, StringComparison.Ordinal)).Any())
        {
            throw new ValidationException("Block structure can only be changed in the default language.");
        }
    }

    private static void ValidateTranslatedBlockLabels(IList<Block> blocks, IList<BlockModel> submittedBlocks)
    {
        var current = new Dictionary<Guid, string>();
        foreach (var block in blocks)
        {
            AddBlockLabels(current, block);
        }

        var submitted = new Dictionary<Guid, string>();
        foreach (var block in submittedBlocks ?? new List<BlockModel>())
        {
            AddBlockLabels(submitted, block);
        }

        if (current.Count != submitted.Count || current.Any(block =>
            !submitted.TryGetValue(block.Key, out var label) ||
            !string.Equals(block.Value, label, StringComparison.Ordinal)))
        {
            throw new ValidationException("Block labels can only be changed in the default language.");
        }
    }

    private static void AddBlockStructure(IList<BlockStructure> structure, Block block, Guid? parentId)
    {
        structure.Add(new BlockStructure(block.Id, parentId, block.Type));

        if (block is BlockGroup group)
        {
            foreach (var child in group.Items)
            {
                AddBlockStructure(structure, child, block.Id);
            }
        }
    }

    private static void AddBlockStructure(IList<BlockStructure> structure, BlockModel block, Guid? parentId)
    {
        switch (block)
        {
            case BlockGroupModel group:
                structure.Add(new BlockStructure(group.Id, parentId, group.Type));
                foreach (var child in group.Items)
                {
                    AddBlockStructure(structure, child, group.Id);
                }
                break;
            case BlockItemModel item when item.Model != null:
                structure.Add(new BlockStructure(item.Model.Id, parentId, item.Model.Type));
                break;
            case BlockGenericModel generic:
                structure.Add(new BlockStructure(generic.Id, parentId, generic.Type));
                break;
            default:
                throw new ValidationException("Invalid block structure.");
        }
    }

    private static void AddBlockLabels(IDictionary<Guid, string> labels, Block block)
    {
        labels.Add(block.Id, NormalizeBlockLabel(block.Label));

        if (block is BlockGroup group)
        {
            foreach (var child in group.Items)
            {
                AddBlockLabels(labels, child);
            }
        }
    }

    private static void AddBlockLabels(IDictionary<Guid, string> labels, BlockModel block)
    {
        switch (block)
        {
            case BlockGroupModel group:
                labels.Add(group.Id, NormalizeBlockLabel(group.Label));
                foreach (var child in group.Items)
                {
                    AddBlockLabels(labels, child);
                }
                break;
            case BlockItemModel item when item.Model != null:
                labels.Add(item.Model.Id, NormalizeBlockLabel(item.Model.Label));
                break;
            case BlockGenericModel generic:
                labels.Add(generic.Id, NormalizeBlockLabel(generic.Label));
                break;
            default:
                throw new ValidationException("Invalid block metadata.");
        }
    }

    private static string NormalizeBlockLabel(string label)
    {
        if (string.IsNullOrWhiteSpace(label))
        {
            return null;
        }

        label = label.Trim();
        if (label.Length > 128)
        {
            throw new ValidationException("Block labels cannot exceed 128 characters.");
        }
        return label;
    }

    private static void MergeDraftBlockStructure(DynamicPage translatedPage, DynamicPage defaultDraft)
    {
        var translatedBlocks = translatedPage.Blocks
            .ToDictionary(block => block.Id);

        translatedPage.Blocks = defaultDraft.Blocks
            .Select(block => MergeDraftBlock(block, translatedBlocks.TryGetValue(block.Id, out var translated) ? translated : null))
            .ToList();
    }

    private static Block MergeDraftBlock(Block draft, Block translated)
    {
        var block = translated ?? CloneDraftBlock(draft);
        block.Label = draft.Label;

        if (draft is BlockGroup draftGroup && block is BlockGroup translatedGroup)
        {
            var translatedItems = translatedGroup.Items.ToDictionary(item => item.Id);
            translatedGroup.Items = draftGroup.Items
                .Select(item => MergeDraftBlock(item, translatedItems.TryGetValue(item.Id, out var translatedItem) ? translatedItem : null))
                .ToList();
        }

        return block;
    }

    private static Block CloneDraftBlock(Block source)
    {
        var clone = (Block)Activator.CreateInstance(source.GetType());
        clone.Id = source.Id;
        clone.Type = source.Type;
        clone.Label = source.Label;

        foreach (var prop in source.GetType().GetProperties(App.PropertyBindings))
        {
            if (typeof(IField).IsAssignableFrom(prop.PropertyType))
            {
                var field = prop.GetValue(source) as IField;
                if (field == null)
                {
                    prop.SetValue(clone, Activator.CreateInstance(prop.PropertyType));
                }
                else if (field is ITranslatable)
                {
                    // New blocks are shared structurally, but their translated
                    // fields must start empty instead of copying the default value.
                    prop.SetValue(clone, Activator.CreateInstance(prop.PropertyType));
                }
                else
                {
                    prop.SetValue(clone, field);
                }
            }
        }

        return clone;
    }

    private static void ValidateTranslationLanguages(IList<Language> languages, Guid sourceLanguageId, Guid targetLanguageId)
    {
        if (sourceLanguageId == targetLanguageId)
        {
            throw new ValidationException("The source and target language must be different.");
        }
        if (!languages.Any(language => language.Id == sourceLanguageId) ||
            !languages.Any(language => language.Id == targetLanguageId))
        {
            throw new ValidationException("The translation file references a language that is not configured for this site.");
        }
    }

    private static PageTranslationExchangeLanguage MapExchangeLanguage(Language language)
    {
        return new PageTranslationExchangeLanguage
        {
            Id = language.Id,
            Title = language.Title,
            Culture = language.Culture
        };
    }

    private static string GetTranslationStructureHash(PageEditModel model)
    {
        var structure = CreateTranslationUnits(model, model)
            .Select(unit => $"{unit.Key}|{unit.FieldType}");
        var content = Encoding.UTF8.GetBytes(string.Join("\n", structure));
        return Convert.ToHexString(SHA256.HashData(content)).ToLowerInvariant();
    }

    private static List<PageTranslationExchangeUnit> CreateTranslationUnits(PageEditModel source, PageEditModel target)
    {
        var units = new List<PageTranslationExchangeUnit>();

        AddMetadataUnits(source, target, units);

        foreach (var sourceRegion in source.Regions)
        {
            var targetRegion = target.Regions.FirstOrDefault(region => region.Meta.Id == sourceRegion.Meta.Id);
            for (var itemIndex = 0; itemIndex < sourceRegion.Items.Count; itemIndex++)
            {
                var sourceItem = sourceRegion.Items[itemIndex];
                var targetItem = targetRegion?.Items.ElementAtOrDefault(itemIndex);

                foreach (var sourceField in sourceItem.Fields)
                {
                    var targetField = targetItem?.Fields.FirstOrDefault(field => field.Meta.Id == sourceField.Meta.Id);
                    AddTextUnit(units,
                        $"region:{sourceRegion.Meta.Id}:{itemIndex}:{sourceField.Meta.Id}",
                        $"{sourceRegion.Meta.Name} / {sourceField.Meta.Name}",
                        sourceField,
                        targetField);
                }
            }
        }

        foreach (var sourceBlock in source.Blocks)
        {
            var sourceBlockId = GetBlockId(sourceBlock);
            var targetBlock = target.Blocks.FirstOrDefault(block => GetBlockId(block) == sourceBlockId);
            AddBlockTextUnits(units, sourceBlock, targetBlock, null);
        }
        return units;
    }

    private static void AddMetadataUnits(PageEditModel source, PageEditModel target, IList<PageTranslationExchangeUnit> units)
    {
        AddMetadataUnit(units, "title", "Page title", source.Title, target.Title);
        AddMetadataUnit(units, "navigationTitle", "Navigation title", source.NavigationTitle, target.NavigationTitle);
        AddMetadataUnit(units, "slug", "Slug", source.Slug, target.Slug);
        AddMetadataUnit(units, "metaTitle", "Meta title", source.MetaTitle, target.MetaTitle);
        AddMetadataUnit(units, "metaKeywords", "Meta keywords", source.MetaKeywords, target.MetaKeywords);
        AddMetadataUnit(units, "metaDescription", "Meta description", source.MetaDescription, target.MetaDescription);
        AddMetadataUnit(units, "ogTitle", "Open Graph title", source.OgTitle, target.OgTitle);
        AddMetadataUnit(units, "ogDescription", "Open Graph description", source.OgDescription, target.OgDescription);
        AddMetadataUnit(units, "excerpt", "Excerpt", source.Excerpt, target.Excerpt);
    }

    private static void AddMetadataUnit(IList<PageTranslationExchangeUnit> units, string key, string context, string source, string target)
    {
        units.Add(new PageTranslationExchangeUnit
        {
            Key = $"metadata:{key}",
            Context = context,
            FieldType = "metadata",
            Source = source,
            Target = target
        });
    }

    private static void AddBlockTextUnits(IList<PageTranslationExchangeUnit> units, BlockModel sourceBlock, BlockModel targetBlock, Guid? parentId)
    {
        var blockId = GetBlockId(sourceBlock);
        var prefix = $"block:{parentId?.ToString() ?? "root"}:{blockId}";
        var sourceFields = GetBlockFields(sourceBlock);
        var targetFields = targetBlock == null ? null : GetBlockFields(targetBlock);

        foreach (var sourceField in sourceFields)
        {
            var targetField = targetFields?.FirstOrDefault(field => field.Meta.Id == sourceField.Meta.Id);
            AddTextUnit(units,
                $"{prefix}:{sourceField.Meta.Id}",
                $"{GetBlockContext(sourceBlock)} / {sourceField.Meta.Name}",
                sourceField,
                targetField);
        }

        if (sourceBlock is BlockGroupModel sourceGroup)
        {
            foreach (var sourceChild in sourceGroup.Items)
            {
                var sourceChildId = GetBlockId(sourceChild);
                var targetChild = (targetBlock as BlockGroupModel)?.Items
                    .FirstOrDefault(block => GetBlockId(block) == sourceChildId);
                AddBlockTextUnits(units, sourceChild, targetChild, blockId);
            }
        }
    }

    private static IList<FieldModel> GetBlockFields(BlockModel block)
    {
        return block switch
        {
            BlockGroupModel group => group.Fields,
            BlockGenericModel generic => generic.Model,
            BlockItemModel item when item.Model != null => ContentUtils.GetBlockFields(item.Model),
            _ => new List<FieldModel>()
        };
    }

    private static Guid GetBlockId(BlockModel block)
    {
        return block switch
        {
            BlockGroupModel group => group.Id,
            BlockGenericModel generic => generic.Id,
            BlockItemModel item when item.Model != null => item.Model.Id,
            _ => Guid.Empty
        };
    }

    private static string GetBlockContext(BlockModel block)
    {
        return block switch
        {
            BlockGroupModel group => group.Label ?? group.Meta.Name,
            BlockGenericModel generic => generic.Label ?? generic.Meta.Name,
            BlockItemModel item => item.Model?.Label ?? item.Meta.Name,
            _ => "Block"
        };
    }

    private static void AddTextUnit(IList<PageTranslationExchangeUnit> units, string key, string context, FieldModel source, FieldModel target)
    {
        if (source?.Model is not SimpleField<string> sourceField)
        {
            return;
        }

        units.Add(new PageTranslationExchangeUnit
        {
            Key = key,
            Context = context,
            FieldType = source.Model.GetType().FullName,
            Source = sourceField.Value,
            Target = (target?.Model as SimpleField<string>)?.Value
        });
    }

    private static Dictionary<string, WritableTranslationUnit> GetWritableTranslationUnits(PageEditModel target)
    {
        var writable = new Dictionary<string, WritableTranslationUnit>();

        AddWritableMetadata(writable, "title", value => target.Title = value);
        AddWritableMetadata(writable, "navigationTitle", value => target.NavigationTitle = value);
        AddWritableMetadata(writable, "slug", value => target.Slug = value);
        AddWritableMetadata(writable, "metaTitle", value => target.MetaTitle = value);
        AddWritableMetadata(writable, "metaKeywords", value => target.MetaKeywords = value);
        AddWritableMetadata(writable, "metaDescription", value => target.MetaDescription = value);
        AddWritableMetadata(writable, "ogTitle", value => target.OgTitle = value);
        AddWritableMetadata(writable, "ogDescription", value => target.OgDescription = value);
        AddWritableMetadata(writable, "excerpt", value => target.Excerpt = value);

        foreach (var region in target.Regions)
        {
            for (var itemIndex = 0; itemIndex < region.Items.Count; itemIndex++)
            {
                foreach (var field in region.Items[itemIndex].Fields)
                {
                    AddWritableField(writable, $"region:{region.Meta.Id}:{itemIndex}:{field.Meta.Id}", field);
                }
            }
        }

        foreach (var block in target.Blocks)
        {
            AddWritableBlockFields(writable, block, null);
        }
        return writable;
    }

    private static void AddWritableMetadata(IDictionary<string, WritableTranslationUnit> writable, string key, Action<string> setValue)
    {
        writable.Add($"metadata:{key}", new WritableTranslationUnit("metadata", setValue));
    }

    private static void AddWritableBlockFields(IDictionary<string, WritableTranslationUnit> writable, BlockModel block, Guid? parentId)
    {
        var blockId = GetBlockId(block);
        var prefix = $"block:{parentId?.ToString() ?? "root"}:{blockId}";
        foreach (var field in GetBlockFields(block))
        {
            AddWritableField(writable, $"{prefix}:{field.Meta.Id}", field);
        }

        if (block is BlockGroupModel group)
        {
            foreach (var child in group.Items)
            {
                AddWritableBlockFields(writable, child, blockId);
            }
        }
    }

    private static void AddWritableField(IDictionary<string, WritableTranslationUnit> writable, string key, FieldModel field)
    {
        if (field?.Model is SimpleField<string> textField)
        {
            writable.Add(key, new WritableTranslationUnit(field.Model.GetType().FullName, value => textField.Value = value));
        }
    }

    private sealed class WritableTranslationUnit
    {
        public WritableTranslationUnit(string fieldType, Action<string> setValue)
        {
            FieldType = fieldType;
            SetValue = setValue;
        }

        public string FieldType { get; }
        public Action<string> SetValue { get; }
    }

    private sealed class BlockStructure
    {
        public BlockStructure(Guid id, Guid? parentId, string type)
        {
            Id = id;
            ParentId = parentId;
            Type = type;
        }

        public Guid Id { get; }
        public Guid? ParentId { get; }
        public string Type { get; }
    }

    private PageListModel.PageItem MapRecursive(Guid siteId, SitemapItem item, int level, int expandedLevels, IEnumerable<Guid> drafts)
    {
        var model = new PageListModel.PageItem
        {
            Id = item.Id,
            SiteId = siteId,
            Title = item.MenuTitle,
            TypeName = item.PageTypeName,
            Published = item.Published.HasValue ? item.Published.Value.ToString("yyyy-MM-dd") : null,
            Status = drafts.Contains(item.Id) ? _localizer.General[PageListModel.PageItem.Draft] :
                !item.Published.HasValue ? _localizer.General[PageListModel.PageItem.Unpublished] : "",
            EditUrl = "manager/page/edit/",
            IsExpanded = level < expandedLevels,
            IsCopy = item.OriginalPageId.HasValue,
            IsRestricted = item.Permissions.Count > 0,
            IsScheduled = item.Published.HasValue && item.Published.Value > DateTime.Now,
            IsUnpublished = !item.Published.HasValue,
            Permalink = item.Permalink
        };

        foreach (var child in item.Items)
        {
            model.Items.Add(MapRecursive(siteId, child, level + 1, expandedLevels, drafts));
        }
        return model;
    }

    private PageEditModel Transform(DynamicPage page, bool isDraft)
    {
        var config = new Config(_api);
        var type = App.PageTypes.GetById(page.TypeId);
        var route = type.Routes.FirstOrDefault(r => r.Route == page.Route) ?? type.Routes.FirstOrDefault();

        var model = new PageEditModel
        {
            Id = page.Id,
            SiteId = page.SiteId,
            ParentId = page.ParentId,
            OriginalId = page.OriginalPageId,
            SortOrder = page.SortOrder,
            TypeId = page.TypeId,
            Title = page.Title,
            NavigationTitle = page.NavigationTitle,
            Slug = page.Slug,
            MetaTitle = page.MetaTitle,
            MetaKeywords = page.MetaKeywords,
            MetaDescription = page.MetaDescription,
            MetaIndex = page.MetaIndex,
            MetaFollow = page.MetaFollow,
            MetaPriority = page.MetaPriority,
            OgTitle = page.OgTitle,
            OgDescription = page.OgDescription,
            OgImage = page.OgImage,
            PrimaryImage = page.PrimaryImage,
            Excerpt = page.Excerpt,
            IsHidden = page.IsHidden,
            IsScheduled = page.Published.HasValue && page.Published.Value > DateTime.Now,
            Published = page.Published.HasValue ? page.Published.Value.ToString("yyyy-MM-dd") : null,
            PublishedTime = page.Published.HasValue ? page.Published.Value.ToString("HH:mm") : null,
            RedirectUrl = page.RedirectUrl,
            RedirectType = page.RedirectType.ToString(),
            EnableComments = page.EnableComments,
            CloseCommentsAfterDays = page.CloseCommentsAfterDays,
            CommentCount = page.CommentCount,
            State = page.GetState(isDraft),
            UseBlocks = type.UseBlocks,
            UsePrimaryImage = type.UsePrimaryImage,
            UseExcerpt = type.UseExcerpt,
            UseHtmlExcerpt = config.HtmlExcerpt,
            SelectedRoute = route == null ? null : new RouteModel
            {
                Title = route.Title,
                Route = route.Route
            },
            Permissions = App.Permissions
                .GetPublicPermissions()
                .Select(p => new KeyValuePair<string, string>(p.Name, p.Title))
                .ToList(),
            SelectedPermissions = page.Permissions
        };

        foreach (var r in type.Routes)
        {
            model.Routes.Add(new RouteModel {
                Title = r.Title,
                Route = r.Route
            });
        }

        foreach (var regionType in type.Regions)
        {
            var region = new RegionModel
            {
                Meta = new RegionMeta
                {
                    Id = regionType.Id,
                    Name = regionType.Title,
                    Description = regionType.Description,
                    Placeholder = regionType.ListTitlePlaceholder,
                    IsCollection = regionType.Collection,
                    Expanded = regionType.ListExpand,
                    Icon = regionType.Icon,
                    Display = regionType.Display.ToString().ToLower(),
                    Width = regionType.Width.ToString().ToLower()
                }
            };
            var regionListModel = ((IDictionary<string, object>)page.Regions)[regionType.Id];

            if (!regionType.Collection)
            {
                var regionModel = (IRegionList)Activator.CreateInstance(typeof(RegionList<>).MakeGenericType(regionListModel.GetType()));
                regionModel.Add(regionListModel);
                regionListModel = regionModel;
            }

            foreach (var regionModel in (IEnumerable)regionListModel)
            {
                var regionItem = new RegionItemModel();

                foreach (var fieldType in regionType.Fields)
                {
                    var appFieldType = App.Fields.GetByType(fieldType.Type);

                    var field = new FieldModel
                    {
                        Meta = new FieldMeta
                        {
                            Id = fieldType.Id,
                            Name = fieldType.Title,
                            Component = appFieldType.Component,
                            Placeholder = fieldType.Placeholder,
                            IsHalfWidth = fieldType.Options.HasFlag(FieldOption.HalfWidth),
                            Description = fieldType.Description,
                            Settings = fieldType.Settings
                        }
                    };

                    if (typeof(SelectFieldBase).IsAssignableFrom(appFieldType.Type))
                    {
                        foreach(var item in ((SelectFieldBase)Activator.CreateInstance(appFieldType.Type)).Items)
                        {
                            field.Meta.Options.Add(Convert.ToInt32(item.Value), item.Title);
                        }
                    }

                    if (regionType.Fields.Count > 1)
                    {
                        field.Model = (IField)((IDictionary<string, object>)regionModel)[fieldType.Id];

                        if (regionType.ListTitleField == fieldType.Id)
                        {
                            regionItem.Title = field.Model.GetTitle();
                            field.Meta.NotifyChange = true;
                        }
                    }
                    else
                    {
                        field.Model = (IField)regionModel;
                        field.Meta.NotifyChange = true;
                        regionItem.Title = field.Model.GetTitle();
                    }
                    regionItem.Fields.Add(field);
                }

                if (string.IsNullOrWhiteSpace(regionItem.Title))
                {
                    regionItem.Title = "...";
                }

                region.Items.Add(regionItem);
            }
            model.Regions.Add(region);
        }

        foreach (var block in page.Blocks)
        {
            var blockType = App.Blocks.GetByType(block.Type);

            if (block is BlockGroup)
            {
                var group = new BlockGroupModel
                {
                    Id = block.Id,
                    Label = block.Label,
                    Type = block.Type,
                    Meta = new BlockMeta
                    {
                        Name = blockType.Name,
                        Icon = blockType.Icon,
                        Component = blockType.Component,
                        Width = blockType.Width.ToString().ToLower(),
                        IsGroup = true,
                        IsReadonly = page.OriginalPageId.HasValue,
                        isCollapsed = config.ManagerDefaultCollapsedBlocks,
                        ShowHeader = !config.ManagerDefaultCollapsedBlockGroupHeaders
                    }
                };

                group.Fields = ContentUtils.GetBlockFields(block);

                bool firstChild = true;
                foreach (var child in ((BlockGroup)block).Items)
                {
                    blockType = App.Blocks.GetByType(child.Type);

                    if (!blockType.IsGeneric)
                    {
                        // Regular block item model
                        group.Items.Add(new BlockItemModel
                        {
                            IsActive = firstChild,
                            Model = child,
                            Meta = new BlockMeta
                            {
                                Name = blockType.Name,
                                Title = child.GetTitle(),
                                Icon = blockType.Icon,
                                Component = blockType.Component,
                                Width = blockType.Width.ToString().ToLower()
                            }
                        });
                    }
                    else
                    {
                        // Generic block item model
                        group.Items.Add(new BlockGenericModel
                        {
                            Id = child.Id,
                            Label = child.Label,
                            IsActive = firstChild,
                            Model = ContentUtils.GetBlockFields(child),
                            Type = child.Type,
                            Meta = new BlockMeta
                            {
                                Name = blockType.Name,
                                Title = child.GetTitle(),
                                Icon = blockType.Icon,
                                Component = blockType.Component,
                                Width = blockType.Width.ToString().ToLower()
                            }
                        });
                    }
                    firstChild = false;
                }
                model.Blocks.Add(group);
            }
            else
            {
                if (!blockType.IsGeneric)
                {
                    // Regular block item model
                    model.Blocks.Add(new BlockItemModel
                    {
                        Model = block,
                        Meta = new BlockMeta
                        {
                            Name = blockType.Name,
                            Title = block.GetTitle(),
                            Icon = blockType.Icon,
                            Component = blockType.Component,
                            Width = blockType.Width.ToString().ToLower(),
                            IsReadonly = page.OriginalPageId.HasValue,
                            isCollapsed = config.ManagerDefaultCollapsedBlocks
                        }
                    });
                }
                else
                {
                    // Generic block item model
                    model.Blocks.Add(new BlockGenericModel
                    {
                        Id = block.Id,
                        Label = block.Label,
                        Model = ContentUtils.GetBlockFields(block),
                        Type = block.Type,
                        Meta = new BlockMeta
                        {
                            Name = blockType.Name,
                            Title = block.GetTitle(),
                            Icon = blockType.Icon,
                            Component = blockType.Component,
                            Width = blockType.Width.ToString().ToLower(),
                            IsReadonly = page.OriginalPageId.HasValue,
                            isCollapsed = config.ManagerDefaultCollapsedBlocks
                        }
                    });
                }
            }
        }

        // Custom editors
        foreach (var editor in type.CustomEditors)
        {
            model.Editors.Add(new EditorModel
            {
                Component = editor.Component,
                Icon = editor.Icon,
                Name = editor.Title
            });
        }
        return model;
    }

    private DateTime? ParsePublishedDate(PageEditModel model)
    {
        if (!string.IsNullOrEmpty(model.Published))
        {
            var str = model.Published.Substring(0, 10);

            if (!string.IsNullOrEmpty(model.PublishedTime))
            {
                str += $" { model.PublishedTime }";
            }
            return DateTime.Parse(str);
        }
        return null;
    }
}
