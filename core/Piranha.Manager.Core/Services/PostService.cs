/*
 * Copyright (c) 2019 Håkan Edling
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

        public async Task<PostListModel> GetList(Guid archiveId)
        {
            var model = new PostListModel
            {
                PostTypes = App.PostTypes.Select(t => new PostListModel.PostTypeItem
                {
                    Id = t.Id,
                    Title = t.Title,
                    AddUrl = "manager/post/add/"
                }).ToList()
            };

            // Get posts
            model.Posts = (await _api.Posts.GetAllAsync<PostInfo>(archiveId))
                .Select(p => new PostListModel.PostItem
                {
                    Id = p.Id.ToString(),
                    Title = p.Title,
                    TypeName = model.PostTypes.First(t => t.Id == p.TypeId).Title,
                    Category = p.Category.Title,
                    Published = p.Published.HasValue ? p.Published.Value.ToString("yyyy-MM-dd HH:mm") : null,
                    Status = GetState(p, false),
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
            //var page = useDraft ? await _api.Posts.GetDraftByIdAsync(id) : null;
            var post = useDraft ? await _api.Posts.GetByIdAsync(id) : null;

            if (post == null)
            {
                post = await _api.Posts.GetByIdAsync(id);
                isDraft = false;
            }

            if (post != null)
            {
                var postModel =  Transform(post, isDraft);

                postModel.Categories = (await _api.Posts.GetAllCategoriesAsync(post.BlogId))
                    .Select(c => c.Title).ToList();
                postModel.Tags = (await _api.Posts.GetAllTagsAsync(post.BlogId))
                    .Select(t => t.Title).ToList();

                postModel.SelectedCategory = post.Category.Title;
                postModel.SelectedTags = post.Tags.Select(t => t.Title).ToList();

                return postModel;
            }
            return null;
        }

        private PostEditModel Transform(DynamicPost post, bool isDraft)
        {
            var type = App.PostTypes.GetById(post.TypeId);

            var model = new PostEditModel
            {
                Id = post.Id,
                BlogId = post.BlogId,
                TypeId = post.TypeId,
                Title = post.Title,
                Slug = post.Slug,
                MetaKeywords = post.MetaKeywords,
                MetaDescription = post.MetaDescription,
                Published = post.Published.HasValue ? post.Published.Value.ToString("yyyy-MM-dd HH:mm") : null,
                State = "published", //GetState(post, isDraft),
                UseBlocks = type.UseBlocks
            };

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
                            IsGroup = true
                        }
                    };

                    if (blockType.Display != BlockDisplayMode.MasterDetail)
                    {
                        group.Meta.Component = blockType.Display == BlockDisplayMode.Horizontal ?
                            "block-group-horizontal" : "block-group-vertical";
                    }

                    foreach (var prop in block.GetType().GetProperties(App.PropertyBindings))
                    {
                        if (typeof(Extend.IField).IsAssignableFrom(prop.PropertyType))
                        {
                            var fieldType = App.Fields.GetByType(prop.PropertyType);

                            group.Fields.Add(new FieldModel
                            {
                                Model = (Extend.IField)prop.GetValue(block),
                                Meta = new FieldMeta
                                {
                                    Id = prop.Name,
                                    Name = prop.Name,
                                    Component = fieldType.Component,
                                }
                            });
                        }
                    }

                    bool firstChild = true;
                    foreach (var child in ((Extend.BlockGroup)block).Items)
                    {
                        blockType = App.Blocks.GetByType(child.Type);

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
                        firstChild = false;
                    }
                    model.Blocks.Add(group);
                }
                else
                {
                    model.Blocks.Add(new BlockItemModel
                    {
                        Model = block,
                        Meta = new BlockMeta
                        {
                            Name = blockType.Name,
                            Title = block.GetTitle(),
                            Icon = blockType.Icon,
                            Component = blockType.Component
                        }
                    });
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

        private string GetState(PostBase post, bool isDraft)
        {
            if (post.Created != DateTime.MinValue)
            {
                if (post.Published.HasValue)
                {
                    if (isDraft)
                    {
                        return ContentState.Draft;
                    }
                    return ContentState.Published;
                }
                else
                {
                    return ContentState.Unpublished;
                }
            }
            return ContentState.New;
        }
    }
}