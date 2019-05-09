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
using System.Linq;
using System.Threading.Tasks;
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

                var items = App.Blocks.GetByCategory(category).OrderBy(i => i.Name).Where(i => !i.IsUnlisted);

                // If we have a parent, filter on allowed types
                if (parent != null)
                {
                    if (parent.ItemTypes.Count > 0)
                    {
                        items = items.Where(i => parent.ItemTypes.Contains(i.Type));
                    }

                    if (exludeGroups)
                    {
                        items = items.Where(i => !typeof(Piranha.Extend.BlockGroup).IsAssignableFrom(i.Type));
                    }
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

        public RegionItemModel CreatePageRegion(string type, string region)
        {
            var pageType = App.PageTypes.GetById(type);

            if (pageType != null)
            {
                var regionType = pageType.Regions.First(r => r.Id == region);
                var regionModel = _factory.CreateDynamicRegion(pageType, region);
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
                            IsHalfWidth = fieldType.Options.HasFlag(FieldOption.HalfWidth)
                        }
                    };

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
            return null;
        }

        /// <summary>
        /// Creates a new block of the specified type.
        /// </summary>
        /// <param name="type">The block type</param>
        /// <returns>The new block</returns>
        public BlockModel CreateBlock(string type)
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

                            item.Fields.Add(new FieldModel
                            {
                                Model = (Extend.IField)prop.GetValue(block),
                                Meta = new FieldMeta
                                {
                                    Name = prop.Name,
                                    Component = fieldType.Component,
                                }
                            });
                        }
                    }

                    return item;
                }
                else
                {
                    return new BlockItemModel
                    {
                        Model = block,
                        Meta = new BlockMeta
                        {
                            Name = blockType.Name,
                            Title = block.GetTitle(),
                            Icon = blockType.Icon,
                            Component = blockType.Component
                        }
                    };
                }
            }
            return null;
        }
    }
}
