/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Collections;
using System.Dynamic;
using AutoMapper;
using Piranha.Data;
using Piranha.Models;

namespace Piranha.Services;

/// <inheritdoc />
internal class ContentService<TContent, TField, TModelBase> : IContentService<TContent, TField, TModelBase>
    where TContent : ContentBase<TField>
    where TField : ContentFieldBase
    where TModelBase : ContentBase
{
    //
    // Members
    protected readonly IContentFactory _factory;
    protected readonly IMapper _mapper;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="factory">The content factory</param>
    /// <param name="mapper">The AutoMapper instance to use</param>
    public ContentService(IContentFactory factory, IMapper mapper)
    {
        _factory = factory;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<T> TransformAsync<T>(TContent content, Models.ContentTypeBase type, Func<TContent, T, Task> process = null, Guid? languageId = null)
        where T : Models.ContentBase, TModelBase
    {
        if (content is Content genericContent)
        {
            // We need the currently selected language id for mapping.
            genericContent.SelectedLanguageId = languageId;
        }

        if (type != null)
        {
            //
            // 1: Get the requested model type
            //
            var modelType = typeof(T);
            if (!typeof(Models.IDynamicContent).IsAssignableFrom(modelType) && !typeof(Models.IContentInfo).IsAssignableFrom(modelType))
            {
                modelType = Type.GetType(type.CLRType);

                if (modelType != typeof(T) && !typeof(T).IsAssignableFrom(modelType))
                {
                    return null;
                }
            }

            //
            // 2: Create an initialized model
            //
            var model = await _factory.CreateAsync<T>(type);

            //
            // 3: Map basic fields
            //
            _mapper.Map<TContent, TModelBase>(content, model);

            //
            // 4: Map routes
            //
            if (model is Models.RoutedContentBase routeModel)
            {
                // Map route (if available)
                if (string.IsNullOrWhiteSpace(routeModel.Route) && type.Routes.Count > 0)
                    routeModel.Route = type.Routes.First();
            }

            //
            // 5: Map translation
            //
            if (content is ITranslatable translatableContent && languageId.HasValue)
            {
                var translation = translatableContent.GetTranslation(languageId.Value);

                if (translation != null)
                {
                    _mapper.Map(translation, model);
                }
            }

            //
            // 6: Map regions
            //
            if (!(model is IContentInfo))
            {
                var currentRegions = type.Regions.Select(r => r.Id).ToArray();

                foreach (var regionKey in currentRegions)
                {
                    var region = type.Regions.Single(r => r.Id == regionKey);
                    var fields = content.Fields.Where(f => f.RegionId == regionKey).OrderBy(f => f.SortOrder).ToList();

                    if (!region.Collection)
                    {
                        foreach (var fieldDef in region.Fields)
                        {
                            var field = fields.SingleOrDefault(f => f.FieldId == fieldDef.Id && f.SortOrder == 0);

                            if (field != null)
                            {
                                if (region.Fields.Count == 1)
                                {
                                    SetSimpleValue(model, regionKey, field, languageId);
                                    break;
                                }
                                else
                                {
                                    SetComplexValue(model, regionKey, fieldDef.Id, field, languageId);
                                }
                            }
                        }
                    }
                    else
                    {
                        var fieldCount = content.Fields.Where(f => f.RegionId == regionKey).Select(f => f.SortOrder).DefaultIfEmpty(-1).Max() + 1;
                        var sortOrder = 0;

                        while (fieldCount > sortOrder)
                        {
                            if (region.Fields.Count == 1)
                            {
                                var field = fields.SingleOrDefault(f => f.FieldId == region.Fields[0].Id && f.SortOrder == sortOrder);
                                if (field != null)
                                    AddSimpleValue(model, regionKey, field, languageId);
                            }
                            else
                            {
                                await AddComplexValueAsync(model, type, regionKey, fields.Where(f => f.SortOrder == sortOrder).ToList(), languageId)
                                    .ConfigureAwait(false);
                            }
                            sortOrder++;
                        }
                    }
                }
            }

            if (process != null)
            {
                await process(content, model).ConfigureAwait(false);
            }

            return model;
        }
        return null;
    }

    /// <inheritdoc />
    public TContent Transform<T>(T model, Models.ContentTypeBase type, TContent dest = null, Guid? languageId = null)
        where T : Models.ContentBase, TModelBase
    {
        var content = dest == null ? Activator.CreateInstance<TContent>() : dest;

        //
        // 1: Map id
        //
        if (model.Id != Guid.Empty)
        {
            content.Id = model.Id;
        }
        else
        {
            content.Id = model.Id = Guid.NewGuid();
        }
        content.Created = DateTime.Now;

        //
        // 2: Map basic fields
        //
        _mapper.Map<TModelBase, TContent>(model, content);

        //
        // 3: Map translation
        //
        if (content is ITranslatable translatableContent && languageId.HasValue)
        {
            translatableContent.SetTranslation(content.Id, languageId.Value, model);
        }

        //
        // 4: Map category
        //
        if (model is Models.ICategorizedContent categorized)
        {
            if (content is ICategorized categorizedContent)
            {
                if (type is Models.ContentType contentType)
                {
                    if (contentType.UseCategory)
                    {
                        categorizedContent.CategoryId = categorized.Category.Id;
                    }
                }
                else
                {
                    categorizedContent.CategoryId = categorized.Category.Id;
                }
            }
        }

        //
        // 5: Map regions
        //
        var currentRegions = type.Regions.Select(r => r.Id).ToArray();

        foreach (var regionKey in currentRegions)
        {
            // Check that the region exists in the current model
            if (HasRegion(model, regionKey))
            {
                var regionType = type.Regions.Single(r => r.Id == regionKey);

                if (!regionType.Collection)
                {
                    MapRegion(content, GetRegion(model, regionKey), regionType, regionKey, languageId: languageId);
                }
                else
                {
                    var items = new List<Guid>();
                    var sortOrder = 0;
                    foreach (var region in GetEnumerable(model, regionKey))
                    {
                        var fields = MapRegion(content, region, regionType, regionKey, sortOrder++, languageId);

                        if (fields.Count > 0)
                            items.AddRange(fields);
                    }

                    // Now delete removed collection items
                    var removedFields = content.Fields
                        .Where(f => f.RegionId == regionKey && !items.Contains(f.Id))
                        .ToList();

                    foreach (var removed in removedFields)
                    {
                        content.Fields.Remove(removed);
                    }
                }
            }
        }
        return content;
    }

    /// <inheritdoc />
    public IList<Extend.Block> TransformBlocks(IEnumerable<Block> blocks)
    {
        var models = new List<Extend.Block>();

        foreach (var block in blocks)
        {
            var blockType = App.Blocks.GetByType(block.CLRType);

            if (blockType != null)
            {
                var model = (Extend.Block)Activator.CreateInstance(blockType.Type);
                model.Id = block.Id;
                model.Type = block.CLRType;

                foreach (var prop in model.GetType().GetProperties(App.PropertyBindings))
                {
                    if (typeof(Extend.IField).IsAssignableFrom(prop.PropertyType))
                    {
                        var field = block.Fields.FirstOrDefault(f => f.FieldId == prop.Name);

                        if (field != null)
                        {
                            var type = App.Fields.GetByType(field.CLRType);
                            var val = (Extend.IField)App.DeserializeObject(field.Value, type.Type);

                            prop.SetValue(model, val);
                        }
                        else
                        {
                            prop.SetValue(model, Activator.CreateInstance(prop.PropertyType));
                        }
                    }
                }

                if (block.ParentId.HasValue)
                {
                    var parent = models.FirstOrDefault(m => m.Id == block.ParentId.Value);

                    if (parent != null && typeof(Extend.BlockGroup).IsAssignableFrom(parent.GetType()))
                    {
                        ((Extend.BlockGroup)parent).Items.Add(model);
                    }
                    else
                    {
                        throw new InvalidOperationException("Block parent is missing");
                    }
                }
                else
                {
                    models.Add(model);
                }
            }
        }
        return models;
    }

    /// <inheritdoc />
    public IList<Extend.Block> TransformBlocks(IEnumerable<ContentBlock> blocks, Guid? languageId)
    {
        var models = new List<Extend.Block>();

        foreach (var block in blocks)
        {
            var blockType = App.Blocks.GetByType(block.CLRType);

            if (blockType != null)
            {
                var model = (Extend.Block)Activator.CreateInstance(blockType.Type);
                model.Id = block.Id;
                model.Type = block.CLRType;

                foreach (var prop in model.GetType().GetProperties(App.PropertyBindings))
                {
                    if (typeof(Extend.IField).IsAssignableFrom(prop.PropertyType))
                    {
                        var field = block.Fields.FirstOrDefault(f => f.FieldId == prop.Name);

                        if (field != null)
                        {
                            prop.SetValue(model, DeserializeValue(field, languageId));
                        }
                        else
                        {
                            prop.SetValue(model, Activator.CreateInstance(prop.PropertyType));
                        }
                    }
                }

                if (block.ParentId.HasValue)
                {
                    var parent = models.FirstOrDefault(m => m.Id == block.ParentId.Value);

                    if (parent != null && typeof(Extend.BlockGroup).IsAssignableFrom(parent.GetType()))
                    {
                        ((Extend.BlockGroup)parent).Items.Add(model);
                    }
                    else
                    {
                        throw new InvalidOperationException("Block parent is missing");
                    }
                }
                else
                {
                    models.Add(model);
                }
            }
        }
        return models;
    }

    /// <inheritdoc />
    public IList<Block> TransformBlocks(IList<Extend.Block> models)
    {
        var blocks = new List<Block>();

        if (models != null)
        {
            for (var n = 0; n < models.Count; n++)
            {
                var type = App.Blocks.GetByType(models[n].GetType().FullName);

                if (type != null)
                {
                    var block = new Block()
                    {
                        Id = models[n].Id != Guid.Empty ? models[n].Id : Guid.NewGuid(),
                        CLRType = models[n].GetType().FullName,
                        Created = DateTime.Now,
                        LastModified = DateTime.Now
                    };

                    foreach (var prop in models[n].GetType().GetProperties(App.PropertyBindings))
                    {
                        if (typeof(Extend.IField).IsAssignableFrom(prop.PropertyType))
                        {
                            // Only save fields to the database
                            var field = new BlockField()
                            {
                                Id = Guid.NewGuid(),
                                BlockId = block.Id,
                                FieldId = prop.Name,
                                SortOrder = 0,
                                CLRType = prop.PropertyType.FullName,
                                Value = App.SerializeObject(prop.GetValue(models[n]), prop.PropertyType)
                            };
                            block.Fields.Add(field);
                        }
                    }
                    blocks.Add(block);

                    if (typeof(Extend.BlockGroup).IsAssignableFrom(models[n].GetType()))
                    {
                        var blockItems = TransformBlocks(((Extend.BlockGroup)models[n]).Items);

                        if (blockItems.Count() > 0)
                        {
                            foreach (var item in blockItems)
                            {
                                item.ParentId = block.Id;
                            }
                            blocks.AddRange(blockItems);
                        }
                    }
                }
            }
        }
        return blocks;
    }

    /// <inheritdoc />
    public IList<ContentBlock> TransformContentBlocks(IList<Extend.Block> models, Guid languageId)
    {
        var blocks = new List<ContentBlock>();

        if (models != null)
        {
            for (var n = 0; n < models.Count; n++)
            {
                var type = App.Blocks.GetByType(models[n].GetType().FullName);

                if (type != null)
                {
                    // Make sure we generate an id if it's empty
                    if (models[n].Id == Guid.Empty)
                    {
                        models[n].Id = Guid.NewGuid();
                    }

                    var block = new ContentBlock()
                    {
                        Id = models[n].Id,
                        CLRType = models[n].GetType().FullName
                    };

                    foreach (var prop in models[n].GetType().GetProperties(App.PropertyBindings))
                    {
                        if (typeof(Extend.IField).IsAssignableFrom(prop.PropertyType))
                        {
                            var blockValue = prop.GetValue(models[n]);

                            // Only save fields to the database
                            var field = new ContentBlockField()
                            {
                                Id = Guid.NewGuid(),
                                BlockId = block.Id,
                                FieldId = prop.Name,
                                SortOrder = 0,
                                CLRType = prop.PropertyType.FullName,
                            };

                            // Set value
                            if (blockValue is Extend.ITranslatable)
                            {
                                var translation = new ContentBlockFieldTranslation
                                {
                                    FieldId = field.Id,
                                    LanguageId = languageId,
                                    Value = App.SerializeObject(blockValue, prop.PropertyType)
                                };
                                field.Translations.Add(translation);
                            }
                            else
                            {
                                field.Value = App.SerializeObject(blockValue, prop.PropertyType);
                            }
                            block.Fields.Add(field);
                        }
                    }
                    blocks.Add(block);

                    if (typeof(Extend.BlockGroup).IsAssignableFrom(models[n].GetType()))
                    {
                        var blockItems = TransformContentBlocks(((Extend.BlockGroup)models[n]).Items, languageId);

                        if (blockItems.Count() > 0)
                        {
                            foreach (var item in blockItems)
                            {
                                item.ParentId = block.Id;
                            }
                            blocks.AddRange(blockItems);
                        }
                    }
                }
            }
        }
        return blocks;
    }

    /// <summary>
    /// Gets the enumerator for the given region collection.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    /// <param name="model">The model</param>
    /// <param name="regionId">The region id</param>
    /// <returns>The enumerator</returns>
    private IEnumerable GetEnumerable<T>(T model, string regionId) where T : Models.ContentBase
    {
        object value = null;

        if (model is Models.IDynamicContent dynamicModel)
        {
            value = ((IDictionary<string, object>)dynamicModel.Regions)[regionId];
        }
        else
        {
            value = model.GetType().GetProperty(regionId, App.PropertyBindings).GetValue(model);
        }
        if (value is IEnumerable)
            return (IEnumerable)value;
        return null;
    }

    /// <summary>
    /// Gets the region with the given key.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    /// <param name="model">The model</param>
    /// <param name="regionId">The region id</param>
    /// <returns>The region</returns>
    private object GetRegion<T>(T model, string regionId) where T : Models.ContentBase
    {
        if (model is Models.IDynamicContent dynamicModel)
        {
            return ((IDictionary<string, object>)dynamicModel.Regions)[regionId];
        }
        else
        {
            return model.GetType().GetProperty(regionId, App.PropertyBindings).GetValue(model);
        }
    }

    /// <summary>
    /// Checks if the given model has a region with the specified id.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    /// <param name="model">The model</param>
    /// <param name="regionId">The region id</param>
    /// <returns>If the region exists</returns>
    private bool HasRegion<T>(T model, string regionId) where T : Models.ContentBase
    {
        if (model is Models.IDynamicContent dynamicModel)
        {
            return ((IDictionary<string, object>)dynamicModel.Regions).ContainsKey(regionId);
        }
        else
        {
            return model.GetType().GetProperty(regionId, App.PropertyBindings) != null;
        }
    }

    /// <summary>
    /// Maps a region to the given data entity.
    /// </summary>
    /// <param name="content">The content entity</param>
    /// <param name="region">The region to map</param>
    /// <param name="regionType">The region type</param>
    /// <param name="regionId">The region id</param>
    /// <param name="sortOrder">The optional sort order</param>
    /// <param name="languageId">The optional language id</param>
    private IList<Guid> MapRegion(TContent content, object region, Models.ContentTypeRegion regionType, string regionId, int sortOrder = 0, Guid? languageId = null)
    {
        var items = new List<Guid>();

        // Now map all of the fields
        for (var n = 0; n < regionType.Fields.Count; n++)
        {
            var fieldDef = regionType.Fields[n];
            var fieldType = App.Fields.GetByShorthand(fieldDef.Type);
            if (fieldType == null)
                fieldType = App.Fields.GetByType(fieldDef.Type);

            if (fieldType != null)
            {
                object fieldValue = null;
                if (regionType.Fields.Count == 1)
                {
                    // Get the field value for simple region
                    fieldValue = region;
                }
                else
                {
                    // Get the field value for complex region
                    fieldValue = GetComplexValue(region, fieldDef.Id);
                }

                if (fieldValue != null)
                {
                    // Check that the returned value matches the type specified
                    // for the page type, otherwise deserialization won't work
                    // when the model is retrieved from the database.
                    if (fieldValue.GetType() != fieldType.Type)
                        throw new ArgumentException("Given field value does not match the configured type");

                    // Check if we have the current field in the database already
                    var field = content.Fields
                        .SingleOrDefault(f => f.RegionId == regionId && f.FieldId == fieldDef.Id && f.SortOrder == sortOrder);

                    // If not, create a new field
                    if (field == null)
                    {
                        field = Activator.CreateInstance<TField>();
                        field.Id = Guid.NewGuid();
                        field.RegionId = regionId;
                        field.FieldId = fieldDef.Id;

                        content.Fields.Add(field);
                    }

                    // Update field info
                    field.CLRType = fieldType.TypeName;
                    field.SortOrder = sortOrder;

                    // Update field value
                    if (fieldValue is Extend.ITranslatable && field is ITranslatable translatableField && languageId.HasValue)
                    {
                        // This is a translatable value
                        translatableField.SetTranslation(field.Id, languageId.Value, fieldValue);
                    }
                    else
                    {
                        // this is a non translatable valuye
                        field.Value = App.SerializeObject(fieldValue, fieldType.Type);
                    }
                    items.Add(field.Id);
                }
            }
        }
        return items;
    }

    /// <summary>
    /// Sets the value of a simple single field region.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    /// <param name="model">The model</param>
    /// <param name="regionId">The region id</param>
    /// <param name="field">The field</param>
    /// <param name="languageId">The languageId</param>
    private void SetSimpleValue<T>(T model, string regionId, TField field, Guid? languageId) where T : Models.ContentBase
    {
        if (model is Models.IDynamicContent dynamicModel)
        {
            ((IDictionary<string, object>)dynamicModel.Regions)[regionId] =
                DeserializeValue(field, languageId);
        }
        else
        {
            var regionProp = model.GetType().GetProperty(regionId, App.PropertyBindings);

            if (regionProp != null)
            {
                regionProp.SetValue(model, DeserializeValue(field, languageId));
            }
        }
    }

    /// <summary>
    /// Adds a simple single field value to a collection region.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    /// <param name="model">The model</param>
    /// <param name="regionId">The region id</param>
    /// <param name="field">The field</param>
    /// <param name="languageId">The languageId</param>
    private void AddSimpleValue<T>(T model, string regionId, TField field, Guid? languageId) where T : Models.ContentBase
    {
        if (model is Models.IDynamicContent dynamicModel)
        {
            ((IList)((IDictionary<string, object>)dynamicModel.Regions)[regionId]).Add(
                DeserializeValue(field, languageId));
        }
        else
        {
            var regionProp = model.GetType().GetProperty(regionId, App.PropertyBindings);

            if (regionProp != null)
            {
                ((IList)regionProp.GetValue(model)).Add(DeserializeValue(field, languageId));
            }
        }
    }

    /// <summary>
    /// Sets the value of a complex region.
    /// </summary>
    /// <typeparam name="T">The model</typeparam>
    /// <param name="model">The model</param>
    /// <param name="regionId">The region id</param>
    /// <param name="fieldId">The field id</param>
    /// <param name="field">The field</param>
    /// <param name="languageId">The languageId</param>
    private void SetComplexValue<T>(T model, string regionId, string fieldId, TField field, Guid? languageId)
        where T : Models.ContentBase
    {
        if (model is Models.IDynamicContent dynamicModel)
        {
            ((IDictionary<string, object>)((IDictionary<string, object>)dynamicModel.Regions)[regionId])[fieldId] =
                DeserializeValue(field, languageId);
        }
        else
        {
            var regionProp = model.GetType().GetProperty(regionId, App.PropertyBindings);

            if (regionProp != null)
            {
                var obj = regionProp.GetValue(model);
                if (obj != null)
                {
                    var fieldProp = obj.GetType().GetProperty(fieldId, App.PropertyBindings);

                    if (fieldProp != null)
                    {
                        fieldProp.SetValue(obj, DeserializeValue(field, languageId));
                    }
                }
            }
        }
    }

    /// <summary>
    /// Adds a complex region to a collection region.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    /// <param name="model">The model</param>
    /// <param name="contentType">The content type</param>
    /// <param name="regionId">The region id</param>
    /// <param name="fields">The field</param>
    /// <param name="languageId">The languageId</param>
    private async Task AddComplexValueAsync<T>(T model, Models.ContentTypeBase contentType, string regionId, IList<TField> fields, Guid? languageId)
        where T : Models.ContentBase
    {
        if (fields.Count > 0)
        {
            if (model is Models.IDynamicContent dynamicModel)
            {
                var list = (IList)((IDictionary<string, object>)dynamicModel.Regions)[regionId];
                var obj = await _factory.CreateDynamicRegionAsync(contentType, regionId);

                foreach (var field in fields)
                {
                    if (((IDictionary<string, object>)obj).ContainsKey(field.FieldId))
                    {
                        ((IDictionary<string, object>)obj)[field.FieldId] =
                            DeserializeValue(field, languageId);
                    }
                }
                list.Add(obj);
            }
            else
            {
                var regionProp = model.GetType().GetProperty(regionId, App.PropertyBindings);

                if (regionProp != null)
                {
                    var list = (IList)regionProp.GetValue(model);
                    var obj = Activator.CreateInstance(list.GetType().GenericTypeArguments.First());

                    foreach (var field in fields)
                    {
                        var fieldProp = obj.GetType().GetProperty(field.FieldId, App.PropertyBindings);
                        if (fieldProp != null)
                        {
                            fieldProp.SetValue(obj, DeserializeValue(field, languageId));
                        }
                    }
                    list.Add(obj);
                }
            }
        }
    }

    /// <summary>
    /// Deserializes the given field value.
    /// </summary>
    /// <param name="field">The page field</param>
    /// <param name="languageId">The optional language id</param>
    /// <returns>The value</returns>
    private object DeserializeValue(TField field, Guid? languageId)
    {
        var type = App.Fields.GetByType(field.CLRType);

        if (type != null)
        {
            if (typeof(Extend.ITranslatable).IsAssignableFrom(type.Type) && field is ITranslatable translatable && languageId.HasValue)
            {
                return App.DeserializeObject((string)translatable.GetTranslation(languageId.Value), type.Type);
            }
            return App.DeserializeObject(field.Value, type.Type);
        }
        return null;
    }

    /// <summary>
    /// Deserializes the given field value.
    /// </summary>
    /// <param name="field">The page field</param>
    /// <param name="languageId">The optional language id</param>
    /// <returns>The value</returns>
    private object DeserializeValue(BlockFieldBase field, Guid? languageId)
    {
        var type = App.Fields.GetByType(field.CLRType);

        if (type != null)
        {
            if (typeof(Extend.ITranslatable).IsAssignableFrom(type.Type) && field is ITranslatable translatable && languageId.HasValue)
            {
                return App.DeserializeObject((string)translatable.GetTranslation(languageId.Value), type.Type);
            }
            return App.DeserializeObject(field.Value, type.Type);
        }
        return null;
    }

    /// <summary>
    /// Gets a field value from a complex region.
    /// </summary>
    /// <param name="region">The region</param>
    /// <param name="fieldId">The field id</param>
    /// <returns>The value</returns>
    private object GetComplexValue(object region, string fieldId)
    {
        if (region is ExpandoObject)
        {
            return ((IDictionary<string, object>)region)[fieldId];
        }
        else
        {
            var fieldProp = region.GetType().GetProperty(fieldId, App.PropertyBindings);

            if (fieldProp != null)
            {
                return fieldProp.GetValue(region);
            }
            return null;
        }
    }
}
