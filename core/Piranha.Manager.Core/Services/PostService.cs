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
using Piranha.Extend;
using Piranha.Manager.Extensions;
using Piranha.Models;
using Piranha.Manager.Models;
using Piranha.Manager.Models.Content;
using Piranha.Services;

namespace Piranha.Manager.Services
{
    public class PostService
    {
        private readonly IApi _api;
        private readonly IContentFactory _factory;

        public PostService(IApi api, IContentFactory factory)
        {
            _api = api;
            _factory = factory;
        }

        public async Task<PostModalModel> GetArchiveMap(Guid? siteId, Guid? archiveId)
        {
            var model = new PostModalModel();

            // Get default site if none is selected
            if (!siteId.HasValue)
            {
                var site = await _api.Sites.GetDefaultAsync();
                if (site != null)
                {
                    siteId = site.Id;
                }
            }

            model.SiteId = siteId.Value;

            // Get the sites available
            model.Sites = (await _api.Sites.GetAllAsync())
                .Select(s => new PostModalModel.SiteItem
                {
                    Id = s.Id,
                    Title = s.Title
                })
                .OrderBy(s => s.Title)
                .ToList();

            // Get the current site title
            var currentSite = model.Sites.FirstOrDefault(s => s.Id == siteId.Value);
            if (currentSite != null)
            {
                model.SiteTitle = currentSite.Title;
            }

            // Get the blogs available
            model.Archives = (await _api.Pages.GetAllBlogsAsync<PageInfo>(siteId.Value))
                .Select(p => new PostModalModel.ArchiveItem
                {
                    Id = p.Id,
                    Title = p.Title,
                    Slug = p.Slug
                })
                .OrderBy(p => p.Title)
                .ToList();

            if (model.Archives.Any())
            {
                if (!archiveId.HasValue)
                {
                    // Select the first blog
                    archiveId = model.Archives.First().Id;
                }

                var archive = model.Archives.FirstOrDefault(b => b.Id == archiveId.Value);
                if (archive != null)
                {
                    model.ArchiveId = archive.Id;
                    model.ArchiveTitle = archive.Title;
                    model.ArchiveSlug = archive.Slug;
                }

                // Get the available posts
                model.Posts = (await _api.Posts.GetAllAsync<PostInfo>(archiveId.Value))
                    .Select(p => new PostModalModel.PostModalItem
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Permalink = "/" + model.ArchiveSlug + "/" + p.Slug,
                        Published = p.Published?.ToString("yyyy-MM-dd HH:mm")
                    }).ToList();

                // Sort so we show unpublished drafts first
                model.Posts = model.Posts.Where(p => string.IsNullOrEmpty(p.Published))
                    .Concat(model.Posts.Where(p => !string.IsNullOrEmpty(p.Published)))
                    .ToList();
            }

            return model;
        }

        public async Task<PostListModel> GetList(Guid archiveId, int index = 0)
        {
            var page = await _api.Pages.GetByIdAsync<PageInfo>(archiveId);
            if (page == null)
            {
                return new PostListModel();
            }

            var pageType = App.PageTypes.GetById(page.TypeId);
            var pageSize = 0;

            using (var config = new Config(_api))
            {
                pageSize = config.ManagerPageSize;
            }

            var model = new PostListModel
            {
                PostTypes = App.PostTypes.Select(t => new PostListModel.PostTypeItem
                {
                    Id = t.Id,
                    Title = t.Title,
                    AddUrl = "manager/post/add/"
                }).ToList(),
                TotalPosts = await _api.Posts.GetCountAsync(archiveId)
            };

            model.TotalPages = Convert.ToInt32(Math.Ceiling(model.TotalPosts / Convert.ToDouble(pageSize)));
            model.Index = index;

            // We have specified the post types that should be available
            // in this archive. Filter them accordingly
            if (pageType.ArchiveItemTypes.Count > 0)
            {
                model.PostTypes = model.PostTypes
                    .Where(t => pageType.ArchiveItemTypes.Contains(t.Id))
                    .ToList();
            }

            // Get drafts
            var drafts = await _api.Posts.GetAllDraftsAsync(archiveId);

            // Get posts
            model.Posts = (await _api.Posts.GetAllAsync<PostInfo>(archiveId, index, pageSize))
                .Select(p => new PostListModel.PostItem
                {
                    Id = p.Id.ToString(),
                    Title = p.Title,
                    TypeName = model.PostTypes.First(t => t.Id == p.TypeId).Title,
                    Category = p.Category.Title,
                    Published = p.Published?.ToString("yyyy-MM-dd HH:mm"),
                    Status = p.GetState(drafts.Contains(p.Id)),
                    isScheduled = p.Published.HasValue && p.Published.Value > DateTime.Now,
                    EditUrl = "manager/post/edit/"
                }).ToList();

            // Get categories
            model.Categories = (await _api.Posts.GetAllCategoriesAsync(archiveId))
                .Select(c => new PostListModel.CategoryItem
                {
                    Id = c.Id.ToString(),
                    Title = c.Title
                }).ToList();

            return model;
        }

        public async Task<PostEditModel> GetById(Guid id, bool useDraft = true)
        {
            var isDraft = true;
            var post = useDraft ? await _api.Posts.GetDraftByIdAsync(id) : null;

            if (post == null)
            {
                post = await _api.Posts.GetByIdAsync(id);
                isDraft = false;
            }

            if (post != null)
            {
                var postModel = Transform(post, isDraft);

                postModel.Categories = (await _api.Posts.GetAllCategoriesAsync(post.BlogId))
                    .Select(c => c.Title).ToList();
                postModel.Tags = (await _api.Posts.GetAllTagsAsync(post.BlogId))
                    .Select(t => t.Title).ToList();
                postModel.PendingCommentCount = (await _api.Posts.GetAllPendingCommentsAsync(id))
                    .Count();

                postModel.SelectedCategory = post.Category.Title;
                postModel.SelectedTags = post.Tags.Select(t => t.Title).ToList();

                return postModel;
            }
            return null;
        }

        public async Task<PostEditModel> Create(Guid archiveId, string typeId)
        {
            var post = await _api.Posts.CreateAsync<DynamicPost>(typeId);

            if (post != null)
            {
                post.Id = Guid.NewGuid();
                post.BlogId = archiveId;

                var postModel = Transform(post, false);

                postModel.Categories = (await _api.Posts.GetAllCategoriesAsync(post.BlogId))
                    .Select(c => c.Title).ToList();
                postModel.Tags = (await _api.Posts.GetAllTagsAsync(post.BlogId))
                    .Select(t => t.Title).ToList();

                postModel.SelectedCategory = post.Category?.Title;
                postModel.SelectedTags = post.Tags.Select(t => t.Title).ToList();

                return postModel;
            }
            return null;
        }

        public async Task Save(PostEditModel model, bool draft)
        {
            var postType = App.PostTypes.GetById(model.TypeId);

            if (postType != null)
            {
                if (model.Id == Guid.Empty)
                {
                    model.Id = Guid.NewGuid();
                }

                var post = await _api.Posts.GetByIdAsync(model.Id);

                if (post == null)
                {
                    post = await _factory.CreateAsync<DynamicPost>(postType);
                    post.Id = model.Id;
                }

                post.BlogId = model.BlogId;
                post.TypeId = model.TypeId;
                post.Title = model.Title;
                post.Slug = model.Slug;
                post.MetaKeywords = model.MetaKeywords;
                post.MetaDescription = model.MetaDescription;
                post.Excerpt = model.Excerpt;
                post.Published = !string.IsNullOrEmpty(model.Published) ? DateTime.Parse(model.Published) : (DateTime?)null;
                post.RedirectUrl = model.RedirectUrl;
                post.RedirectType = (RedirectType)Enum.Parse(typeof(RedirectType), model.RedirectType);
                post.EnableComments = model.EnableComments;
                post.CloseCommentsAfterDays = model.CloseCommentsAfterDays;
                post.Permissions = model.SelectedPermissions;
                post.PrimaryImage = model.PrimaryImage;

                if (postType.Routes.Count > 1)
                {
                    post.Route = postType.Routes.FirstOrDefault(r => r.Route == model.SelectedRoute?.Route)
                                 ?? postType.Routes.First();
                }

                // Save category
                post.Category = new Taxonomy
                {
                    Title = model.SelectedCategory
                };

                // Save tags
                post.Tags.Clear();
                foreach (var tag in model.SelectedTags)
                {
                    post.Tags.Add(new Taxonomy
                    {
                        Title = tag
                    });
                }

                // Save regions
                foreach (var region in postType.Regions)
                {
                    var modelRegion = model.Regions
                        .FirstOrDefault(r => r.Meta.Id == region.Id);

                    if (region.Collection)
                    {
                        var listRegion = (IRegionList)((IDictionary<string, object>)post.Regions)[region.Id];

                        listRegion.Clear();

                        foreach (var item in modelRegion.Items)
                        {
                            if (region.Fields.Count == 1)
                            {
                                listRegion.Add(item.Fields[0].Model);
                            }
                            else
                            {
                                var postRegion = new ExpandoObject();

                                foreach (var field in region.Fields)
                                {
                                    var modelField = item.Fields
                                        .FirstOrDefault(f => f.Meta.Id == field.Id);
                                    ((IDictionary<string, object>)postRegion)[field.Id] = modelField.Model;
                                }
                                listRegion.Add(postRegion);
                            }
                        }
                    }
                    else
                    {
                        var postRegion = ((IDictionary<string, object>)post.Regions)[region.Id];

                        if (region.Fields.Count == 1)
                        {
                            ((IDictionary<string, object>)post.Regions)[region.Id] =
                                modelRegion.Items[0].Fields[0].Model;
                        }
                        else
                        {
                            foreach (var field in region.Fields)
                            {
                                var modelField = modelRegion.Items[0].Fields
                                    .FirstOrDefault(f => f.Meta.Id == field.Id);
                                ((IDictionary<string, object>)postRegion)[field.Id] = modelField.Model;
                            }
                        }
                    }
                }

                // Save blocks
                post.Blocks.Clear();

                foreach (var block in model.Blocks)
                {
                    if (block is BlockGroupModel blockGroup)
                    {
                        var groupType = App.Blocks.GetByType(blockGroup.Type);

                        if (groupType != null)
                        {
                            var postBlock = (Extend.BlockGroup)Activator.CreateInstance(groupType.Type);

                            postBlock.Id = blockGroup.Id;
                            postBlock.Type = blockGroup.Type;

                            foreach (var field in blockGroup.Fields)
                            {
                                var prop = postBlock.GetType().GetProperty(field.Meta.Id, App.PropertyBindings);
                                prop.SetValue(postBlock, field.Model);
                            }

                            foreach (var item in blockGroup.Items)
                            {
                                if (item is BlockItemModel blockItem)
                                {
                                    postBlock.Items.Add(blockItem.Model);
                                }
                                else if (item is BlockGenericModel blockGeneric)
                                {
                                    var transformed = ContentUtils.TransformGenericBlock(blockGeneric);

                                    if (transformed != null)
                                    {
                                        postBlock.Items.Add(transformed);
                                    }
                                }
                            }
                            post.Blocks.Add(postBlock);
                        }
                    }
                    else if (block is BlockItemModel blockItem)
                    {
                        post.Blocks.Add(blockItem.Model);
                    }
                    else if (block is BlockGenericModel blockGeneric)
                    {
                        var transformed = ContentUtils.TransformGenericBlock(blockGeneric);

                        if (transformed != null)
                        {
                            post.Blocks.Add(transformed);
                        }
                    }
                }

                // Save post
                if (draft)
                {
                    await _api.Posts.SaveDraftAsync(post);
                }
                else
                {
                    await _api.Posts.SaveAsync(post);
                }
            }
            else
            {
                throw new ValidationException("Invalid Post Type.");
            }
        }

        /// <summary>
        /// Deletes the post with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public Task Delete(Guid id)
        {
            return _api.Posts.DeleteAsync(id);
        }

        private PostEditModel Transform(DynamicPost post, bool isDraft)
        {
            var config = new Config(_api);
            var type = App.PostTypes.GetById(post.TypeId);
            var route = type.Routes.FirstOrDefault(r => r.Route == post.Route) ?? type.Routes.FirstOrDefault();

            var model = new PostEditModel
            {
                Id = post.Id,
                BlogId = post.BlogId,
                TypeId = post.TypeId,
                PrimaryImage = post.PrimaryImage,
                Title = post.Title,
                Slug = post.Slug,
                MetaKeywords = post.MetaKeywords,
                MetaDescription = post.MetaDescription,
                Excerpt = post.Excerpt,
                Published = post.Published?.ToString("yyyy-MM-dd HH:mm"),
                RedirectUrl = post.RedirectUrl,
                RedirectType = post.RedirectType.ToString(),
                EnableComments = post.EnableComments,
                CloseCommentsAfterDays = post.CloseCommentsAfterDays,
                CommentCount = post.CommentCount,
                State = post.GetState(isDraft),
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
                SelectedPermissions = post.Permissions
            };

            foreach (var r in type.Routes)
            {
                model.Routes.Add(new RouteModel
                {
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
                var regionListModel = ((IDictionary<string, object>)post.Regions)[regionType.Id];

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

            foreach (var block in post.Blocks)
            {
                var blockType = App.Blocks.GetByType(block.Type);

                if (block is BlockGroup blockGroup)
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
                    foreach (var child in blockGroup.Items)
                    {
                        blockType = App.Blocks.GetByType(child.Type);

                        if (!blockType.IsGeneric)
                        {
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