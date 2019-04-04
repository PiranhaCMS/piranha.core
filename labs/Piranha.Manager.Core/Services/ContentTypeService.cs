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

            foreach (var category in App.Blocks.GetCategories())
            {
                var listCategory = new BlockListModel.ListCategory
                {
                    Name = category
                };

                var items = App.Blocks.GetByCategory(category).Where(i => !i.IsUnlisted);

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

        /// <summary>
        /// Creates a new block of the specified type.
        /// </summary>
        /// <param name="type">The block type</param>
        /// <returns>The new block</returns>
        public BlockEditModel CreateBlock(string type)
        {
            var blockType = App.Blocks.GetByType(type);

            if (blockType != null)
            {
                var block = (Extend.Block)_factory.CreateBlock(type);

                if (block is Extend.BlockGroup)
                {
                    var item = new BlockEditModel
                    {
                        Meta = new ContentMeta
                        {
                            Name = blockType.Name,
                            Title = block.GetTitle(),
                            Icon = blockType.Icon,
                            Component = "block-group"
                        }
                    };

                    var groupItem = new BlockGroupEditModel
                    {
                        Type = block.Type
                    };

                    foreach (var prop in block.GetType().GetProperties(App.PropertyBindings))
                    {
                        if (typeof(Extend.IField).IsAssignableFrom(prop.PropertyType))
                        {
                            var fieldType = App.Fields.GetByType(prop.PropertyType);

                            groupItem.Fields.Add(new FieldEditModel
                            {
                                Type = fieldType.TypeName,
                                Model = (Extend.IField)prop.GetValue(block),
                                Meta = new FieldMeta
                                {
                                    Name = prop.Name,
                                    Component = fieldType.Component,
                                }
                            });
                        }
                    }
                    item.Model = groupItem;

                    return item;
                }
                else
                {
                    return new BlockEditModel
                    {
                        Model = block,
                        Meta = new ContentMeta
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
