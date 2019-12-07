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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Piranha.Models;
using Piranha.Manager.Models;
using Piranha.Manager.Models.Content;
using Piranha.Services;

namespace Piranha.Manager.Services
{
    public class ContentTypeService
    {
        private readonly IApi _api;
        private readonly IContentFactory _factory;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="factory">The content factory</param>
        public ContentTypeService(IApi api, IContentFactory factory)
        {
            _api = api;
            _factory = factory;
        }

        /// <summary>
        /// Gets the currently available block types.
        /// </summary>
        /// <param name="parentType">The optional parent group type</param>
        /// <returns>The block list model</returns>
        public BlockListModel GetBlockTypes(string parentType = null)
        {
            var model = new BlockListModel();
            var parent = App.Blocks.GetByType(parentType);
            var exludeGroups = parent != null && typeof(Piranha.Extend.BlockGroup).IsAssignableFrom(parent.Type);

            foreach (var category in App.Blocks.GetCategories().OrderBy(c => c))
            {
                var listCategory = new BlockListModel.ListCategory
                {
                    Name = category
                };

                var items = App.Blocks.GetByCategory(category).OrderBy(i => i.Name).ToList();

                // If we have a parent, filter on allowed types
                if (parent != null)
                {
                    if (parent.ItemTypes.Count > 0)
                    {
                        // Only allow specified types
                        items = items.Where(i => parent.ItemTypes.Contains(i.Type)).ToList();
                    }
                    else
                    {
                        // Remove unlisted types
                        items = items.Where(i => !i.IsUnlisted).ToList();
                    }

                    if (exludeGroups)
                    {
                        items = items.Where(i => !typeof(Piranha.Extend.BlockGroup).IsAssignableFrom(i.Type)).ToList();
                    }
                }
                // Else remove unlisted types
                else
                {
                    items = items.Where(i => !i.IsUnlisted).ToList();
                }

                foreach (var block in items) {
                    listCategory.Items.Add(new BlockListModel.ListItem
                    {
                        Name = block.Name,
                        Icon = block.Icon,
                        Type = block.TypeName
                    });
                }
                model.Categories.Add(listCategory);
            }

            // Remove empty categories
            var empty = model.Categories.Where(c =>  c.Items.Count() == 0).ToList();
            foreach (var remove in empty)
            {
                model.Categories.Remove(remove);
            }

            // Calculate type count
            model.TypeCount = model.Categories.Sum(c => c.Items.Count());

            return model;
        }

        /// <summary>
        /// Creates a new page region.
        /// </summary>
        /// <param name="type">The type id</param>
        /// <param name="region">The region id</param>
        /// <returns>The new region item</returns>
        public RegionItemModel CreatePageRegion(string type, string region)
        {
            var pageType = App.PageTypes.GetById(type);

            if (pageType != null)
            {
                return CreateRegion(pageType, region);
            }
            return null;
        }

        /// <summary>
        /// Creates a new post region.
        /// </summary>
        /// <param name="type">The type id</param>
        /// <param name="region">The region id</param>
        /// <returns>The new region item</returns>
        public RegionItemModel CreatePostRegion(string type, string region)
        {
            var postType = App.PostTypes.GetById(type);

            if (postType != null)
            {
                return CreateRegion(postType, region);
            }
            return null;
        }

        /// <summary>
        /// Creates a new site region.
        /// </summary>
        /// <param name="type">The type id</param>
        /// <param name="region">The region id</param>
        /// <returns>The new region item</returns>
        public RegionItemModel CreateSiteRegion(string type, string region)
        {
            var siteType = App.SiteTypes.GetById(type);

            if (siteType != null)
            {
                return CreateRegion(siteType, region);
            }
            return null;
        }

        /// <summary>
        /// Creates a new region for the given content type.
        /// </summary>
        /// <param name="type">The content type</param>
        /// <param name="region">The region id</param>
        /// <returns>The new region item</returns>
        private RegionItemModel CreateRegion(ContentType type, string region)
        {
            var regionType = type.Regions.First(r => r.Id == region);
            var regionModel = _factory.CreateDynamicRegion(type, region);
            var regionItem = new RegionItemModel
            {
                Title = regionType.ListTitlePlaceholder ?? "..."
            };

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

                PopulateFieldOptions(appFieldType, field);

                if (regionType.Fields.Count > 1)
                {
                    field.Model = (Extend.IField)((IDictionary<string, object>)regionModel)[fieldType.Id];
                    field.Meta.NotifyChange = regionType.ListTitleField == fieldType.Id;
                }
                else
                {
                    field.Model = (Extend.IField)regionModel;
                    field.Meta.NotifyChange = true;
                }
                regionItem.Fields.Add(field);
            }
            return regionItem;
        }

        /// <summary>
        /// Creates a new block of the specified type.
        /// </summary>
        /// <param name="type">The block type</param>
        /// <returns>The new block</returns>
        public AsyncResult<BlockModel> CreateBlock(string type)
        {
            var blockType = App.Blocks.GetByType(type);

            if (blockType != null)
            {
                var block = (Extend.Block)_factory.CreateBlock(type);

                if (block is Extend.BlockGroup)
                {
                    var item = new BlockGroupModel
                    {
                        Type = block.Type,
                        Meta = new BlockMeta
                        {
                            Name = blockType.Name,
                            Title = block.GetTitle(),
                            Icon = blockType.Icon,
                            Component = "block-group",
                            IsGroup = true
                        }
                    };

                    if (blockType.Display != BlockDisplayMode.MasterDetail)
                    {
                        item.Meta.Component = blockType.Display == BlockDisplayMode.Horizontal ?
                            "block-group-horizontal" : "block-group-vertical";
                    }

                    foreach (var prop in block.GetType().GetProperties(App.PropertyBindings))
                    {
                        if (typeof(Extend.IField).IsAssignableFrom(prop.PropertyType))
                        {
                            var fieldType = App.Fields.GetByType(prop.PropertyType);

                            // Create the block field
                            var field = new FieldModel
                            {
                                Model = (Extend.IField)prop.GetValue(block),
                                Meta = new FieldMeta
                                {
                                    Id = prop.Name,
                                    Name = prop.Name,
                                    Component = fieldType.Component,
                                }
                            };

                            PopulateFieldOptions(fieldType, field);

                            // Check if we have field meta-data available
                            var attr = prop.GetCustomAttribute<Extend.FieldAttribute>();
                            if (attr != null)
                            {
                                field.Meta.Name = !string.IsNullOrWhiteSpace(attr.Title) ? attr.Title : field.Meta.Name;
                                field.Meta.Placeholder = attr.Placeholder;
                                field.Meta.IsHalfWidth = attr.Options.HasFlag(FieldOption.HalfWidth);
                            }

                            // Check if we have field description meta-data available
                            var descAttr = prop.GetCustomAttribute<Extend.FieldDescriptionAttribute>();
                            if (descAttr != null)
                            {
                                field.Meta.Description = descAttr.Text;
                            }

                            item.Fields.Add(field);
                        }
                    }

                    return new AsyncResult<BlockModel>
                    {
                        Body = item
                    };
                }
                else
                {
                    return new AsyncResult<BlockModel>
                    {
                        Body = new BlockItemModel
                        {
                            Model = block,
                            Meta = new BlockMeta
                            {
                                Name = blockType.Name,
                                Title = block.GetTitle(),
                                Icon = blockType.Icon,
                                Component = blockType.Component
                            }
                        }
                    };
                }
            }
            return null;
        }

        /// <summary>
        /// Adds options to field's meta if required
        /// </summary>
        /// <param name="fieldType">Type of field</param>
        /// <param name="fieldModel">Field model</param>
        private void PopulateFieldOptions(Runtime.AppField fieldType, FieldModel fieldModel)
        {
            // Check if this is a select field
            if (typeof(Extend.Fields.SelectFieldBase).IsAssignableFrom(fieldType.Type))
            {
                foreach (var selectItem in ((Extend.Fields.SelectFieldBase)Activator.CreateInstance(fieldType.Type)).Items)
                {
                    fieldModel.Meta.Options.Add(Convert.ToInt32(selectItem.Value), selectItem.Title);
                }
            }
        }
    }
}
