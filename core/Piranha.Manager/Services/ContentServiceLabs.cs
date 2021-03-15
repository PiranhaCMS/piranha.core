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
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Manager.Extensions;
using Piranha.Manager.Models;
using Piranha.Manager.Models.Content;
using Piranha.Models;
using Piranha.Services;

namespace Piranha.Manager.Services
{
    public sealed class ContentServiceLabs
    {
        private readonly IApi _api;
        private readonly IContentFactory _factory;
        private readonly TransformationService _transform;
        private readonly Config _config;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="factory">The content factory</param>
        /// <param name="transform">The transformation service</param>
        /// <param name="config">The piranha configuration</param>
        public ContentServiceLabs(IApi api, IContentFactory factory, TransformationService transform, Config config)
        {
            _api = api;
            _factory = factory;
            _transform = transform;
            _config = config;
        }

        public async Task<PageBase> CreatePage(string typeId)
        {
            var type = App.PageTypes.GetById(typeId);
            var modelType = Type.GetType(type.CLRType);

            var create = modelType.GetMethod("CreateAsync", BindingFlags.Public|BindingFlags.Static|BindingFlags.FlattenHierarchy);
            var task = (Task)create.Invoke(null, new object[] { _api, null });

            await task.ConfigureAwait(false);

            var result = task.GetType().GetProperty("Result");
            return (PageBase)result.GetValue(task);
        }

        public async Task<PageBase> ToPage(ContentModel model)
        {
            var page = await CreatePage(model.TypeId);
            return _transform.ToPage(model, page);
        }

        /// <summary>
        /// Gets the page with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The content model</returns>
        public async Task<ContentModel> GetPageByIdAsync(Guid id)
        {
            var isDraft = true;

            // Get the page from the api
            var page = await _api.Pages.GetDraftByIdAsync<PageBase>(id);
            if (page == null)
            {
                page = await _api.Pages.GetByIdAsync<PageBase>(id);
                isDraft = false;
            }

            if (page != null)
            {
                // Get the page type
                var type = App.PageTypes.GetById(page.TypeId);

                // Initialize the page for manager use
                await _factory.InitManagerAsync(page, type);

                // Transform the page
                var model = _transform.Transform(page, type, isDraft);

                // Get the number of pending comments
                model.Comments.PendingCommentCount = (await _api.Posts.GetAllPendingCommentsAsync(id))
                    .Count();

                return model;
            }
            return null;
        }

        /// <summary>
        /// /// Gets the page with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The content model</returns>
        public async Task<ContentModel> GetPostByIdAsync(Guid id)
        {
            var isDraft = true;

            // Get the post from the api
            var post = await _api.Posts.GetDraftByIdAsync<PostBase>(id);
            if (post == null)
            {
                post = await _api.Posts.GetByIdAsync<PostBase>(id);
                isDraft = false;
            }

            if (post != null)
            {
                // Get the post type
                var type = App.PostTypes.GetById(post.TypeId);

                // Initialize the post for manager use
                await _factory.InitManagerAsync(post, type);

                // Transform the post
                var model = _transform.Transform(post, type, isDraft);

                // Get the number of pending comments
                model.Comments.PendingCommentCount = (await _api.Posts.GetAllPendingCommentsAsync(id))
                    .Count();

                // Set the available taxonomies
                model.Taxonomies.Categories = (await _api.Posts.GetAllCategoriesAsync(post.BlogId))
                    .Select(p => p.Title).ToList();
                model.Taxonomies.Tags = (await _api.Posts.GetAllTagsAsync(post.BlogId))
                    .Select(p => p.Title).ToList();

                return model;
            }
            return null;
        }

        public async Task<ContentModel> GetContentByIdAsync(Guid id)
        {
            var content = await _api.Content.GetByIdAsync<GenericContent>(id);

            if (content != null)
            {
                // Get the content type & group
                var type = App.ContentTypes.GetById(content.TypeId);
                var group = App.ContentGroups.GetById(type.Group);

                // Transform the content
                await _factory.InitManagerAsync(content, type);

                // Transform the post
                var model = _transform.Transform(content, type, group);

                return model;
            }
            return null;
        }

        private IList<RegionModel> GetRegions(object model, ContentTypeBase type)
        {
            var result = new List<RegionModel>();

            foreach (var regionType in type.Regions)
            {
                var regionModel = model.GetType().GetProperty(regionType.Id, App.PropertyBindings).GetValue(model);

                var region = new RegionModel
                {
                    Meta = new RegionMeta
                    {
                        Id = regionType.Id,
                        Name = regionType.Title,
                        Icon = regionType.Icon,
                        IsCollection = regionType.Collection,
                        Description = regionType.Description,
                        Placeholder = regionType.ListTitlePlaceholder,
                        Expanded = regionType.ListExpand,
                        Display = regionType.Display.ToString().ToLower()
                    }
                };

                if (!typeof(IEnumerable).IsAssignableFrom(regionModel.GetType()))
                {
                    regionModel = new ArrayList
                    {
                        regionModel
                    };
                }

                foreach (var regionItem in (IEnumerable)regionModel)
                {
                    var item = new RegionItemModel
                    {
                        Fields = GetFields(regionItem, regionType)
                    };

                    if (regionType.Fields.Count == 1)
                    {
                        item.Fields.First().Model = (IField)regionItem;
                        item.Title = ((IField)regionItem).GetTitle();
                    }
                    else
                    {
                        var titleField = item.Fields.FirstOrDefault(f => f.Meta.Id == regionType.ListTitleField);
                        if (titleField != null)
                        {
                            item.Title = ((IField)titleField.Model).GetTitle();
                        }
                    }
                    region.Items.Add(item);
                }

                result.Add(region);
            }
            return result;
        }

        private IList<FieldModel> GetFields(object model, ContentTypeRegion regionType)
        {
            var result = new List<FieldModel>();

            foreach (var fieldType in regionType.Fields)
            {
                var type = App.Fields.GetByType(fieldType.Type);

                var field = new FieldModel
                {
                    Meta = new FieldMeta
                    {
                        Component = type.Component,
                        Description = fieldType.Description,
                        Id = fieldType.Id,
                        IsHalfWidth = fieldType.Options.HasFlag(FieldOption.HalfWidth),
                        Name = fieldType.Title,
                        Placeholder = fieldType.Placeholder,
                        Settings = fieldType.Settings,
                        NotifyChange = fieldType.Id == regionType.ListTitleField
                    }
                };

                if (typeof(SelectFieldBase).IsAssignableFrom(type.Type))
                {
                    foreach(var item in ((SelectFieldBase)Activator.CreateInstance(type.Type)).Items)
                    {
                        field.Meta.Options.Add(Convert.ToInt32(item.Value), item.Title);
                    }
                }

                if (regionType.Fields.Count > 1)
                {
                    // Add the field model
                    var fieldProp = model.GetType().GetProperty(fieldType.Id, App.PropertyBindings);
                    field.Model = (IField)fieldProp.GetValue(model);
                }
                else
                {
                    // We can't add the model here as we don't have the correct
                    // PropertyInfo instance. Calling method will need to handle this.
                    // Let's make sure that this field notifies container of changes.
                    field.Meta.NotifyChange = true;
                }
                result.Add(field);
            }
            return result;
        }

        private IList<BlockModel> GetBlocks(IBlockContent content)
        {
            var result = new List<BlockModel>();

            foreach (var block in content.Blocks)
            {
                var type = App.Blocks.GetByType(block.Type);

                if (block is BlockGroup groupBlock)
                {
                    var group = new BlockGroupModel
                    {
                        Id = groupBlock.Id,
                        Type = groupBlock.Type,
                        Fields = ContentUtils.GetBlockFields(groupBlock),
                        Meta = new BlockMeta
                        {
                            Name = type.Name,
                            Icon = type.Icon,
                            Component = type.Component,
                            Width = type.Width.ToString().ToLower(),
                            IsGroup = true,
                            // TODO: IsReadonly = page.OriginalPageId.HasValue,
                            isCollapsed = _config.ManagerDefaultCollapsedBlocks,
                            ShowHeader = !_config.ManagerDefaultCollapsedBlockGroupHeaders
                        }
                    };

                    foreach (var item in groupBlock.Items)
                    {
                        group.Items.Add(GetBlock(item, App.Blocks.GetByType(item.Type)));
                    }
                    result.Add(group);
                }
                else
                {
                    result.Add(GetBlock(block, type));
                }
            }
            return result;
        }

        private BlockModel GetBlock(Block block, Runtime.AppBlock type)
        {
            if (type.IsGeneric)
            {
                // Generic block model
                return new BlockGenericModel
                {
                    Model = ContentUtils.GetBlockFields(block),
                    Type = block.Type,
                    Meta = new BlockMeta
                    {
                        Name = type.Name,
                        Title = block.GetTitle(),
                        Icon = type.Icon,
                        Component = type.Component,
                        // TODO: IsReadonly = page.OriginalPageId.HasValue,
                        isCollapsed = _config.ManagerDefaultCollapsedBlocks
                    }
                };
            }
            else
            {
                // Regular block model with a unique Vue component
                return new BlockItemModel
                {
                    Model = block,
                    Meta = new BlockMeta
                    {
                        Name = type.Name,
                        Title = block.GetTitle(),
                        Icon = type.Icon,
                        Component = type.Component,
                        // TODO: IsReadonly = page.OriginalPageId.HasValue,
                        isCollapsed = _config.ManagerDefaultCollapsedBlocks
                    }
                };
            }
        }

        private IList<EditorModel> GetEditors(ContentTypeBase type)
        {
            var result = new List<EditorModel>();

            foreach (var editor in type.CustomEditors)
            {
                result.Add(new EditorModel
                {
                    Component = editor.Component,
                    Icon = editor.Icon,
                    Name = editor.Title
                });
            }
            return result;
        }
    }
}