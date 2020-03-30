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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Piranha.Manager.Extensions;
using Piranha.Models;
using Piranha.Manager.Models;
using Piranha.Manager.Models.Content;
using Piranha.Services;

namespace Piranha.Manager.Services
{
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
                    .Select(s => new PageListModel.SiteItem
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
        /// Gets the hierachical page structure for the specified site.
        /// </summary>
        /// <param name="siteId">The site id</param>
        /// <returns>The structure</returns>
        public async Task<List<PageListModel.PageItem>> GetPageStructure(Guid siteId)
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
                    .Select(s => new PageListModel.SiteItem
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

            page.Id = Guid.NewGuid();
            page.SiteId = siteId;
            page.SortOrder = (await _api.Sites.GetSitemapAsync(page.SiteId)).Count;

            if (page != null)
            {
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
                    return Transform(page, false);
                }
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

                    return Transform(page, false);
                }
            }
            return null;
        }

        public async Task<PageEditModel> GetById(Guid id, bool useDraft = true)
        {
            var isDraft = true;
            var page = useDraft ? await _api.Pages.GetDraftByIdAsync(id) : null;

            if (page == null)
            {
                page = await _api.Pages.GetByIdAsync(id);
                isDraft = false;
            }

            if (page != null)
            {
                var model = Transform(page, isDraft);

                model.PendingCommentCount = (await _api.Pages.GetAllPendingCommentsAsync(id))
                    .Count();

                return model;
            }
            return null;
        }

        public async Task<PageEditModel> Detach(Guid id)
        {
            var page = await _api.Pages.GetByIdAsync(id);

            if (page != null)
            {
                await _api.Pages.DetachAsync(page);

                page = await _api.Pages.GetByIdAsync(id);

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

                page.SiteId = model.SiteId;
                page.ParentId = model.ParentId;
                page.OriginalPageId = model.OriginalId;
                page.SortOrder = model.SortOrder;
                page.TypeId = model.TypeId;
                page.Title = model.Title;
                page.NavigationTitle = model.NavigationTitle;
                page.Slug = model.Slug;
                page.MetaKeywords = model.MetaKeywords;
                page.MetaDescription = model.MetaDescription;
                page.IsHidden = model.IsHidden;
                page.Published = !string.IsNullOrEmpty(model.Published) ? DateTime.Parse(model.Published) : (DateTime?)null;
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
                // We only need to save regions & blocks for pages that are not copies
                //
                if (!page.OriginalPageId.HasValue)
                {
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
                                var pageBlock = (Extend.BlockGroup)Activator.CreateInstance(groupType.Type);

                                pageBlock.Id = blockGroup.Id;
                                pageBlock.Type = blockGroup.Type;

                                foreach (var field in blockGroup.Fields)
                                {
                                    var prop = pageBlock.GetType().GetProperty(field.Meta.Id, App.PropertyBindings);
                                    prop.SetValue(pageBlock, field.Model);
                                }

                                foreach (var item in blockGroup.Items)
                                {
                                    if (item is BlockItemModel blockItem)
                                    {
                                        pageBlock.Items.Add(blockItem.Model);
                                    }
                                    else if (item is BlockGenericModel blockGeneric)
                                    {
                                        var transformed = ContentUtils.TransformGenericBlock(blockGeneric);

                                        if (transformed != null)
                                        {
                                            pageBlock.Items.Add(transformed);
                                        }
                                    }
                                }
                                page.Blocks.Add(pageBlock);
                            }
                        }
                        else if (block is BlockItemModel blockItem)
                        {
                            page.Blocks.Add(blockItem.Model);
                        }
                        else if (block is BlockGenericModel blockGeneric)
                        {
                            var transformed = ContentUtils.TransformGenericBlock(blockGeneric);

                            if (transformed != null)
                            {
                                page.Blocks.Add(transformed);
                            }
                        }
                    }
                }

                // Save page
                if (draft)
                {
                    await _api.Pages.SaveDraftAsync(page);
                }
                else
                {
                    await _api.Pages.SaveAsync(page);
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
                MetaKeywords = page.MetaKeywords,
                MetaDescription = page.MetaDescription,
                IsHidden = page.IsHidden,
                Published = page.Published.HasValue ? page.Published.Value.ToString("yyyy-MM-dd HH:mm") : null,
                RedirectUrl = page.RedirectUrl,
                RedirectType = page.RedirectType.ToString(),
                EnableComments = page.EnableComments,
                CloseCommentsAfterDays = page.CloseCommentsAfterDays,
                CommentCount = page.CommentCount,
                State = page.GetState(isDraft),
                UseBlocks = type.UseBlocks,
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
                        Display = regionType.Display.ToString().ToLower()
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
                                Description = fieldType.Description
                            }
                        };

                        if (typeof(Extend.Fields.SelectFieldBase).IsAssignableFrom(appFieldType.Type))
                        {
                            foreach(var item in ((Extend.Fields.SelectFieldBase)Activator.CreateInstance(appFieldType.Type)).Items)
                            {
                                field.Meta.Options.Add(Convert.ToInt32(item.Value), item.Title);
                            }
                        }

                        if (regionType.Fields.Count > 1)
                        {
                            field.Model = (Extend.IField)((IDictionary<string, object>)regionModel)[fieldType.Id];

                            if (regionType.ListTitleField == fieldType.Id)
                            {
                                regionItem.Title = field.Model.GetTitle();
                                field.Meta.NotifyChange = true;
                            }
                        }
                        else
                        {
                            field.Model = (Extend.IField)regionModel;
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

                if (block is Extend.BlockGroup)
                {
                    var group = new BlockGroupModel
                    {
                        Id = block.Id,
                        Type = block.Type,
                        Meta = new BlockMeta
                        {
                            Name = blockType.Name,
                            Icon = blockType.Icon,
                            Component = "block-group",
                            IsGroup = true,
                            IsReadonly = page.OriginalPageId.HasValue,
                            isCollapsed = config.ManagerDefaultCollapsedBlocks,
                            ShowHeader = !config.ManagerDefaultCollapsedBlockGroupHeaders
                        }
                    };

                    if (blockType.Display != BlockDisplayMode.MasterDetail)
                    {
                        group.Meta.Component = blockType.Display == BlockDisplayMode.Horizontal ?
                            "block-group-horizontal" : "block-group-vertical";
                    }

                    group.Fields = ContentUtils.GetBlockFields(block);

                    bool firstChild = true;
                    foreach (var child in ((Extend.BlockGroup)block).Items)
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
                                    Component = blockType.Component
                                }
                            });
                        }
                        else
                        {
                            // Generic block item model
                            group.Items.Add(new BlockGenericModel
                            {
                                IsActive = firstChild,
                                Model = ContentUtils.GetBlockFields(child),
                                Type = child.Type,
                                Meta = new BlockMeta
                                {
                                    Name = blockType.Name,
                                    Title = child.GetTitle(),
                                    Icon = blockType.Icon,
                                    Component = blockType.Component,
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
                            Model = ContentUtils.GetBlockFields(block),
                            Type = block.Type,
                            Meta = new BlockMeta
                            {
                                Name = blockType.Name,
                                Title = block.GetTitle(),
                                Icon = blockType.Icon,
                                Component = blockType.Component,
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
    }
}