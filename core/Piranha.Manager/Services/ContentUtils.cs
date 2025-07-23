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
using Piranha.Manager.Models.Content;
using Piranha.Models;

namespace Piranha.Manager.Services;

public static class ContentUtils
{
    /// <summary>
    /// Gets the fields for the given block.
    /// </summary>
    /// <param name="block">The block</param>
    /// <returns>The available fields</returns>
    public static IList<FieldModel> GetBlockFields(Block block)
    {
        var fields = new List<FieldModel>();

        foreach (var prop in block.GetType().GetProperties(App.PropertyBindings))
        {
            if (typeof(IField).IsAssignableFrom(prop.PropertyType))
            {
                var fieldType = App.Fields.GetByType(prop.PropertyType);
                var field = new FieldModel
                {
                    Model = (IField)prop.GetValue(block),
                    Meta = new FieldMeta
                    {
                        Id = prop.Name,
                        Name = prop.Name,
                        Component = fieldType.Component,
                        IsTranslatable = typeof(ITranslatable).IsAssignableFrom(fieldType.Type),
                        Settings = Utils.GetFieldSettings(prop)
                    }
                };

                // Check if this is a select field
                if (typeof(SelectFieldBase).IsAssignableFrom(fieldType.Type))
                {
                    foreach(var item in ((SelectFieldBase)Activator.CreateInstance(fieldType.Type)).Items)
                    {
                        field.Meta.Options.Add(Convert.ToInt32(item.Value), item.Title);
                    }
                }

                // Check if we have field meta-data available
                var attr = prop.GetCustomAttribute<FieldAttribute>();
                if (attr != null)
                {
                    field.Meta.Name = !string.IsNullOrWhiteSpace(attr.Title) ? attr.Title : field.Meta.Name;
                    field.Meta.Placeholder = attr.Placeholder;
                    field.Meta.IsHalfWidth = attr.Options.HasFlag(FieldOption.HalfWidth);
                    field.Meta.Description = attr.Description;
                }
                fields.Add(field);
            }
        }
        return fields;
    }

    public static Block TransformGenericBlock(BlockGenericModel blockGeneric)
    {
        var blockType = App.Blocks.GetByType(blockGeneric.Type);

        if (blockType != null)
        {
            var pageBlock = (Block)Activator.CreateInstance(blockType.Type);
            pageBlock.Id = blockGeneric.Id;

            foreach (var field in blockGeneric.Model)
            {
                var prop = pageBlock.GetType().GetProperty(field.Meta.Id, App.PropertyBindings);
                prop.SetValue(pageBlock, field.Model);
            }
            return pageBlock;
        }
        return null;
    }
}
