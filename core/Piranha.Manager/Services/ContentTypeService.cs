/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;
using Piranha.Manager.Models;
using Piranha.Manager.Models.Content;
using Piranha.Services;

namespace Piranha.Manager.Services;

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
    /// Gets the currently available block types for the
    /// specified page type.
    /// </summary>
    /// <param name="pageType">The page type id</param>
    /// <param name="parentType">The optional parent group type</param>
    /// <returns>The block list model</returns>
    public BlockListModel GetPageBlockTypes(string pageType, string parentType = null)
    {
        var type = App.PageTypes.GetById(pageType);
        var model = GetBlockTypes(parentType);

        if (type != null && type.BlockItemTypes.Count > 0)
        {
            // First remove all block types that are not allowed
            foreach (var category in model.Categories)
            {
                category.Items = category.Items.Where(i => type.BlockItemTypes.Contains(i.Type)).ToList();
            }

            // Secondly remove all empty categories
            model.Categories = model.Categories.Where(c => c.Items.Count > 0).ToList();
        }
        return model;
    }

    /// <summary>
    /// Gets the currently available block types for the
    /// specified post type.
    /// </summary>
    /// <param name="postType">The post type id</param>
    /// <param name="parentType">The optional parent group type</param>
    /// <returns>The block list model</returns>
    public BlockListModel GetPostBlockTypes(string postType, string parentType = null)
    {
        var type = App.PostTypes.GetById(postType);
        var model = GetBlockTypes(parentType);

        if (type != null && type.BlockItemTypes.Count > 0)
        {
            // First remove all block types that are not allowed
            foreach (var category in model.Categories)
            {
                category.Items = category.Items.Where(i => type.BlockItemTypes.Contains(i.Type)).ToList();
            }

            // Secondly remove all empty categories
            model.Categories = model.Categories.Where(c => c.Items.Count > 0).ToList();
        }
        return model;
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
                listCategory.Items.Add(new BlockListModel.BlockItem
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
    /// Creates a new content region.
    /// </summary>
    /// <param name="type">The type id</param>
    /// <param name="region">The region id</param>
    /// <returns>The new region item</returns>
    public Task<RegionItemModel> CreateContentRegionAsync(string type, string region)
    {
        var pageType = App.ContentTypes.GetById(type);

        if (pageType != null)
        {
            return CreateRegionAsync(pageType, region);
        }
        return null;
    }

    /// <summary>
    /// Creates a new page region.
    /// </summary>
    /// <param name="type">The type id</param>
    /// <param name="region">The region id</param>
    /// <returns>The new region item</returns>
    public Task<RegionItemModel> CreatePageRegionAsync(string type, string region)
    {
        var pageType = App.PageTypes.GetById(type);

        if (pageType != null)
        {
            return CreateRegionAsync(pageType, region);
        }
        return null;
    }

    /// <summary>
    /// Creates a new post region.
    /// </summary>
    /// <param name="type">The type id</param>
    /// <param name="region">The region id</param>
    /// <returns>The new region item</returns>
    public Task<RegionItemModel> CreatePostRegionAsync(string type, string region)
    {
        var postType = App.PostTypes.GetById(type);

        if (postType != null)
        {
            return CreateRegionAsync(postType, region);
        }
        return null;
    }

    /// <summary>
    /// Creates a new site region.
    /// </summary>
    /// <param name="type">The type id</param>
    /// <param name="region">The region id</param>
    /// <returns>The new region item</returns>
    public Task<RegionItemModel> CreateSiteRegionAsync(string type, string region)
    {
        var siteType = App.SiteTypes.GetById(type);

        if (siteType != null)
        {
            return CreateRegionAsync(siteType, region);
        }
        return null;
    }

    /// <summary>
    /// Creates a new region for the given content type.
    /// </summary>
    /// <param name="type">The content type</param>
    /// <param name="region">The region id</param>
    /// <returns>The new region item</returns>
    private async Task<RegionItemModel> CreateRegionAsync(ContentTypeBase type, string region)
    {
        var regionType = type.Regions.First(r => r.Id == region);
        var regionModel = await _factory.CreateDynamicRegionAsync(type, region, true);
        var regionItem = new RegionItemModel
        {
            Title = regionType.ListTitlePlaceholder ?? "...",
            IsNew = true
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
                    Description = fieldType.Description,
                    Settings = fieldType.Settings
                }
            };

            PopulateFieldOptions(appFieldType, field);

            if (regionType.Fields.Count > 1)
            {
                field.Model = (IField)((IDictionary<string, object>)regionModel)[fieldType.Id];
                field.Meta.NotifyChange = regionType.ListTitleField == fieldType.Id;
            }
            else
            {
                field.Model = (IField)regionModel;
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
    public async Task<AsyncResult<BlockModel>> CreateBlockAsync(string type)
    {
        var blockType = App.Blocks.GetByType(type);

        if (blockType != null)
        {
            var block = (Block)(await _factory.CreateBlockAsync(type, true));

            if (block is BlockGroup)
            {
                var item = new BlockGroupModel
                {
                    Type = block.Type,
                    Meta = new BlockMeta
                    {
                        Name = blockType.Name,
                        Title = block.GetTitle(),
                        Icon = blockType.Icon,
                        Component = blockType.Component,
                        Width = blockType.Width.ToString().ToLower(),
                        IsGroup = true
                    }
                };

                item.Fields = ContentUtils.GetBlockFields(block);

                return new AsyncResult<BlockModel>
                {
                    Body = item
                };
            }
            else
            {
                if (!blockType.IsGeneric)
                {
                    // Regular block model
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
                                Component = blockType.Component,
                                Width = blockType.Width.ToString().ToLower()
                            }
                        }
                    };
                }
                else
                {
                    var blockModel = new BlockGenericModel
                    {
                        Model = ContentUtils.GetBlockFields(block),
                        Type = block.Type,
                        Meta = new BlockMeta
                        {
                            Name = blockType.Name,
                            Title = block.GetTitle(),
                            Icon = blockType.Icon,
                            Component = blockType.Component,
                            Width = blockType.Width.ToString().ToLower()
                        }
                    };

                    if (blockModel.Model.Count == 1)
                    {
                        blockModel.Model[0].Meta.NotifyChange = true;
                    }
                    else
                    {
                        foreach (var blockField in blockModel.Model)
                        {
                            blockField.Meta.NotifyChange =
                                blockField.Meta.Id == blockType.ListTitleField;
                        }
                    }

                    // Generic block model
                    return new AsyncResult<BlockModel>
                    {
                        Body = blockModel
                    };
                }
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
        if (typeof(SelectFieldBase).IsAssignableFrom(fieldType.Type))
        {
            foreach (var selectItem in ((SelectFieldBase)Activator.CreateInstance(fieldType.Type)).Items)
            {
                fieldModel.Meta.Options.Add(Convert.ToInt32(selectItem.Value), selectItem.Title);
            }
        }
    }
}
