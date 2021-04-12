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
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Manager.Extensions;
using Piranha.Manager.Models;
using Piranha.Manager.Models.Content;
using Piranha.Models;

namespace Piranha.Manager.Services
{
    public sealed class TransformationService
    {
        private readonly Config _config;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="config">The current piranha config</param>
        public TransformationService(Config config)
        {
            _config = config;
        }

        /// <summary>
        /// Transforms the given page to a content model.
        /// </summary>
        /// <param name="page">The page</param>
        /// <param name="type">The page type</param>
        /// <param name="isDraft">If this is a draft</param>
        /// <returns>The content model</returns>
        public ContentModel ToModel(PageBase page, PageType type, bool isDraft)
        {
            // Map all of the basic fields
            var model = Module.Mapper.Map<PageBase, ContentModel>(page);

            // Set model data
            model.TypeTitle = type.Title;
            model.State = page.GetState(isDraft);
            model.Features = new ContentFeatures
            {
                UseAltTitle = true,
                UseBlocks = type.UseBlocks,
                UseComments = page.EnableComments,
                UseExcerpt = type.UseExcerpt,
                UseHtmlExcerpt = _config.HtmlExcerpt,
                UsePrimaryImage = type.UsePrimaryImage,
                UsePublish = true
            };
            model.Regions = GetRegions(page, type);
            model.Editors = GetEditors(type);
            model.Routes.Routes = GetRoutes(type);
            if (type.UseBlocks)
            {
                model.Blocks = GetBlocks(page);
            }
            return model;
        }

        /// <summary>
        /// Transforms the given post to a content model.
        /// </summary>
        /// <param name="post">The post</param>
        /// <param name="type">The post type</param>
        /// <param name="isDraft">If this is a draft</param>
        /// <returns>The content model</returns>
        public ContentModel ToModel(PostBase post, PostType type, bool isDraft)
        {
            // Map all of the basic fields
            var model = Module.Mapper.Map<PostBase, ContentModel>(post);

            // Set model data
            model.TypeTitle = type.Title;
            model.State = post.GetState(isDraft);
            model.Features = new ContentFeatures
            {
                UseBlocks = type.UseBlocks,
                UseCategory = true,
                UseComments = post.EnableComments,
                UseExcerpt = type.UseExcerpt,
                UseHtmlExcerpt = _config.HtmlExcerpt,
                UsePrimaryImage = type.UsePrimaryImage,
                UsePublish = true,
                UseTags = true
            };
            model.Regions = GetRegions(post, type);
            model.Editors = GetEditors(type);
            model.Routes.Routes = GetRoutes(type);
            if (type.UseBlocks)
            {
                model.Blocks = GetBlocks(post);
            }
            return model;
        }

        /// <summary>
        /// Transforms the given content to a content model.
        /// </summary>
        /// <param name="content">The content</param>
        /// <param name="type">The content type</param>
        /// <param name="group">The content group</param>
        /// <returns>The content model</returns>
        public ContentModel ToModel(GenericContent content, ContentType type, ContentGroup group)
        {
            // Map all of the basic fields
            var model = Module.Mapper.Map<GenericContent, ContentModel>(content);

            // Set model data
            model.TypeTitle = type.Title;
            model.GroupId = group.Id;
            model.GroupTitle = group.Title;
            model.Features = new ContentFeatures
            {
                UseBlocks = type.UseBlocks,
                UseCategory = type.UseCategory,
                UseExcerpt = type.UseExcerpt,
                UseHtmlExcerpt = _config.HtmlExcerpt,
                UsePrimaryImage = type.UsePrimaryImage,
                UseTags = type.UseTags,
                UseTranslations = true
            };
            model.Regions = GetRegions(content, type);
            model.Editors = GetEditors(type);
            if (content is IBlockContent blockContent)
            {
                model.Blocks = GetBlocks(blockContent);
            }

            if (type.UseCategory || type.UseTags)
            {
                model.Taxonomies = new ContentTaxonomies();

                if (content is ICategorizedContent categorizedContent)
                {
                    model.Taxonomies.SelectedCategory = categorizedContent.Category.Title;
                }

                if (content is ITaggedContent taggedContent)
                {
                    model.Taxonomies.SelectedTags = taggedContent.Tags.Select(t => t.Title).ToList();
                }
            }
            return model;
        }

        /// <summary>
        /// Transforms the given content model to a page.
        /// </summary>
        /// <param name="model">The content model</param>
        /// <param name="page">The page</param>
        /// <returns>The transformed page</returns>
        public PageBase ToPage(ContentModel model, PageBase page)
        {
            // Map all of the basic fields
            Module.Mapper.Map<ContentModel, PageBase>(model, page);

            // Set data
            SetRegions(model, page);
            SetBlocks(model, page);

            return page;
        }

        /// <summary>
        /// Transforms the given content model to a post.
        /// </summary>
        /// <param name="model">The content model</param>
        /// <param name="post">The post</param>
        /// <returns>The transformed post</returns>
        public PostBase ToPost(ContentModel model, PostBase post)
        {
            // Map all of the basic fields
            Module.Mapper.Map<ContentModel, PostBase>(model, post);

            // Set data
            SetRegions(model, post);
            SetBlocks(model, post);
            SetCategory(model, post);
            SetTags(model, post);

            return post;
        }

        public GenericContent ToContent(ContentModel model, GenericContent content)
        {
            // Map all of the basic fields
            Module.Mapper.Map<ContentModel, GenericContent>(model, content);

            // Set data
            SetRegions(model, content);
            if (content is IBlockContent blockContent)
            {
                SetBlocks(model, blockContent);
            }
            if (content is ICategorizedContent categoryContent)
            {
                SetCategory(model, categoryContent);
            }
            if (content is ITaggedContent tagContent)
            {
                SetTags(model, tagContent);
            }
            return content;
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

                    for (var n = 0; n < groupBlock.Items.Count; n++)
                    {
                        var item = groupBlock.Items[n];

                        group.Items.Add(GetBlock(item, App.Blocks.GetByType(item.Type), n == 0));
                    }
                    result.Add(group);
                }
                else
                {
                    result.Add(GetBlock(block, type, true));
                }
            }
            return result;
        }

        private BlockModel GetBlock(Block block, Runtime.AppBlock type, bool isActive)
        {
            if (type.IsGeneric)
            {
                // Generic block model
                return new BlockGenericModel
                {
                    IsActive = isActive,
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
                    IsActive = isActive,
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

        private IList<RouteModel> GetRoutes(ContentTypeBase type)
        {
            var result = new List<RouteModel>();

            foreach (var r in type.Routes)
            {
                result.Add(new RouteModel {
                    Title = r.Title,
                    Route = r.Route
                });
            }
            return result;
        }

        private void SetBlocks(ContentModel model, IBlockContent content)
        {
            foreach (var block in model.Blocks)
            {
                if (block is BlockItemModel blockItem)
                {
                    content.Blocks.Add(blockItem.Model);
                }
                else if (block is BlockGroupModel blockGroup)
                {
                    var groupType = App.Blocks.GetByType(blockGroup.Type);

                    if (groupType != null)
                    {
                        var group = (BlockGroup)Activator.CreateInstance(groupType.Type);

                        group.Id = blockGroup.Id;
                        group.Type = blockGroup.Type;

                        foreach (var field in blockGroup.Fields)
                        {
                            var prop = group.GetType().GetProperty(field.Meta.Id, App.PropertyBindings);
                            prop.SetValue(group, field.Model);
                        }

                        foreach (var item in blockGroup.Items)
                        {
                            if (item is BlockItemModel blockGroupItem)
                            {
                                group.Items.Add(blockGroupItem.Model);
                            }
                            else if (item is BlockGenericModel blockGeneric)
                            {
                                var transformed = ContentUtils.TransformGenericBlock(blockGeneric);

                                if (transformed != null)
                                {
                                    group.Items.Add(transformed);
                                }
                            }
                        }
                        content.Blocks.Add(group);
                    }
                }
            }
        }

        private void SetRegions(ContentModel model, ContentBase content)
        {
            foreach (var region in model.Regions)
            {
                var prop = content.GetType().GetProperty(region.Meta.Id, App.PropertyBindings);
                if (prop == null) continue;

                if (region.Meta.IsCollection)
                {
                    IList list = (IList)prop.GetValue(content);

                    foreach (var item in region.Items)
                    {
                        if (item.Fields.Count == 1)
                        {
                            list.Add(item.Fields[0].Model);
                        }
                        else
                        {
                            var obj = Activator.CreateInstance(list.GetType().GetGenericArguments().First());

                            foreach (var field in region.Items[0].Fields)
                            {
                                var fieldProp = obj.GetType().GetProperty(field.Meta.Id, App.PropertyBindings);
                                if (fieldProp == null) continue;

                                fieldProp.SetValue(obj, field.Model);
                            }
                            list.Add(obj);
                        }
                    }
                }
                else
                {
                    if (region.Items[0].Fields.Count == 1)
                    {
                        prop.SetValue(content, region.Items[0].Fields[0].Model);
                    }
                    else
                    {
                        var obj = prop.GetValue(content);

                        foreach (var field in region.Items[0].Fields)
                        {
                            var fieldProp = obj.GetType().GetProperty(field.Meta.Id, App.PropertyBindings);
                            if (fieldProp == null) continue;

                            fieldProp.SetValue(obj, field.Model);
                        }
                    }
                }
            }
        }

        private void SetCategory(ContentModel model, ICategorizedContent content)
        {
            if (model.Taxonomies == null || model.Taxonomies.SelectedCategory == null)
            {
                content.Category = null;
                return;
            }

            content.Category = new Taxonomy
            {
                Title = model.Taxonomies.SelectedCategory
            };
        }

        private void SetTags(ContentModel model, ITaggedContent content)
        {
            // Clear old tags
            content.Tags.Clear();

            if (model.Taxonomies == null || model.Taxonomies.SelectedTags.Count == 0)
            {
                return;
            }

            content.Tags = model.Taxonomies.SelectedTags.Select(t =>
                new Taxonomy
                {
                    Title = t
                }).ToList();
        }
    }
}