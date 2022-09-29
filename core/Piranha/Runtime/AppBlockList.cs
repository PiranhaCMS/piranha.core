/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Reflection;
using Piranha.Extend;
using Piranha.Extend.Fields;

namespace Piranha.Runtime;

public sealed class AppBlockList : AppDataList<Block, AppBlock>
{
    /// <summary>
    /// Gets the block categories sorted in alphabetical order.
    /// </summary>
    /// <returns>The category types</returns>
    public IEnumerable<string> GetCategories()
    {
        return _items
            .Select(i => i.Category)
            .Distinct().OrderBy(c => c)
            .ToArray();
    }

    /// <summary>
    /// Gets the blocks for the given category.
    /// </summary>
    /// <param name="category">The category</param>
    /// <param name="includeGroups">If block groups should be included</param>
    /// <returns>The block types</returns>
    public IEnumerable<AppBlock> GetByCategory(string category, bool includeGroups = true)
    {
        var query = _items
            .Where(i => i.Category == category);

        if (!includeGroups)
        {
            query = query
                .Where(i => !typeof(Extend.BlockGroup).IsAssignableFrom(i.Type));
        }
        return query.ToArray();
    }

    /// <summary>
    /// Performs additional processing on the item before
    /// adding it to the collection.
    /// </summary>
    /// <typeparam name="TValue">The value type</typeparam>
    /// <param name="item">The item</param>
    /// <returns>The processed item</returns>
    protected override AppBlock OnRegister<TValue>(AppBlock item)
    {
        var attr = typeof(TValue).GetCustomAttribute<BlockTypeAttribute>();
        if (attr != null)
        {
            item.Name = attr.Name;
            item.Category = attr.Category;
            item.Icon = attr.Icon;
            item.ListTitleField = attr.ListTitle;
            item.IsUnlisted = attr.IsUnlisted;
            item.IsGeneric = attr.IsGeneric;
            item.Component = !string.IsNullOrWhiteSpace(attr.Component) ? attr.Component : "missing-block";
            item.Width = attr.Width;
            item.Init.InitMethod = Utils.GetMethod<TValue>("Init");
            item.Init.InitManagerMethod = Utils.GetMethod<TValue>("InitManager");

            if (attr is BlockGroupTypeAttribute groupAttr)
            {
                item.Component =
                    groupAttr.Display == Models.BlockDisplayMode.Horizontal ? "block-group-horizontal" :
                    groupAttr.Display == Models.BlockDisplayMode.Vertical ? "block-group-vertical" :
                    "block-group";

                if (!string.IsNullOrWhiteSpace(groupAttr.Component))
                {
                    item.Component = groupAttr.Component;
                }
            }
        }
        else
        {
            throw new CustomAttributeFormatException($"Mandatory attribute missing for registered block { typeof(TValue).Name }");
        }

        var itemAttrs = typeof(TValue).GetCustomAttributes(typeof(BlockItemTypeAttribute));
        foreach (var itemAttr in itemAttrs)
        {
            var itemType = ((BlockItemTypeAttribute)itemAttr).Type;

            // Block groups should not contain items that are
            // other block groups.
            if (!typeof(BlockGroup).IsAssignableFrom(itemType))
            {
                item.ItemTypes.Add(itemType);
            }
        }

        // Automatically register fields for convenience
        foreach (var prop in typeof(TValue).GetProperties(App.PropertyBindings))
        {
            if (typeof(IField).IsAssignableFrom(prop.PropertyType))
            {
                MethodInfo generic = null;

                if (typeof(SelectFieldBase).IsAssignableFrom(prop.PropertyType))
                {
                    var method = typeof(Runtime.AppFieldList).GetMethod("RegisterSelect");
                    generic = method.MakeGenericMethod(prop.PropertyType.GenericTypeArguments.First());
                }
                else
                {
                    var method = typeof(Runtime.AppFieldList).GetMethod("Register");
                    generic = method.MakeGenericMethod(prop.PropertyType);
                }
                generic.Invoke(App.Fields, null);
            }
        }
        return item;
    }
}
