/*
 * Copyright (c) 2018 Håkan Edling
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
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using Piranha.Data;
using Piranha.Models;

namespace Piranha.Services
{
    public class ContentService<TContent, TField, TModelBase> : IContentService<TContent, TField, TModelBase>
        where TContent : Content<TField>
        where TField : ContentField
        where TModelBase : Content
    {
        //
        // Members
        protected readonly IServiceProvider _services;
        protected readonly IMapper _mapper;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="service">The current service provider</param>
        /// <param name="mapper">The AutoMapper instance to use</param>
        public ContentService(IServiceProvider services, IMapper mapper)
        {
            _services = services;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates and initializes a new content model.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="typeId">The content type id</param>
        /// <returns>The model</returns>
        public T Create<T>(Models.ContentType contentType) where T : Models.Content
        {
            using (var scope = _services.CreateScope())
            {
                if (contentType != null)
                {
                    var modelType = typeof(T);

                    if (!typeof(IDynamicModel).IsAssignableFrom(modelType) && !typeof(IContentInfo).IsAssignableFrom(modelType))
                    {
                        modelType = Type.GetType(contentType.CLRType);

                        if (modelType != typeof(T) && !typeof(T).IsAssignableFrom(modelType))
                            return null;
                    }

                    var model = (T)Activator.CreateInstance(modelType);
                    model.TypeId = contentType.Id;

                    if (!(model is IContentInfo))
                    {
                        if (model is IDynamicModel)
                        {
                            var dynModel = (IDynamicModel)(object)model;

                            foreach (var region in contentType.Regions)
                            {
                                object value = null;

                                if (region.Collection)
                                {
                                    var reg = CreateDynamicRegion(scope, region);

                                    if (reg != null)
                                    {
                                        value = Activator.CreateInstance(typeof(RegionList<>).MakeGenericType(reg.GetType()));
                                        ((IRegionList)value).Model = (IDynamicModel)model;
                                        ((IRegionList)value).TypeId = contentType.Id;
                                        ((IRegionList)value).RegionId = region.Id;
                                    }
                                }
                                else
                                {
                                    value = CreateDynamicRegion(scope, region);
                                }

                                if (value != null)
                                    ((IDictionary<string, object>)dynModel.Regions).Add(region.Id, value);
                            }
                        }
                        else
                        {
                            var type = model.GetType();

                            foreach (var region in contentType.Regions)
                            {
                                if (!region.Collection)
                                {
                                    var prop = type.GetProperty(region.Id, App.PropertyBindings);
                                    if (prop != null)
                                    {
                                        prop.SetValue(model, CreateRegion(scope, prop.PropertyType, region));
                                    }
                                }
                            }
                        }
                    }
                    return model;
                }
            }
            return null;
        }

        /// <summary>
        /// Creates a new region.
        /// </summary>
        /// <param name="typeId">The content type id</param>
        /// <param name="regionId">The region id</param>
        /// <returns>The new region value</returns>
        public object CreateDynamicRegion(Models.ContentType contentType, string regionId)
        {
            using (var scope = _services.CreateScope())
            {
                var region = contentType.Regions.SingleOrDefault(r => r.Id == regionId);

                if (region != null)
                    return CreateDynamicRegion(scope, region);
                return null;
            }
        }

        public object CreateBlock(string typeName)
        {
            var blockType = App.Blocks.GetByType(typeName);

            if (blockType != null)
            {
                using (var scope = _services.CreateScope())
                {
                    var block = Activator.CreateInstance(blockType.Type);

                    foreach (var prop in blockType.Type.GetProperties(App.PropertyBindings))
                    {
                        if (typeof(Extend.IField).IsAssignableFrom(prop.PropertyType))
                        {
                            var field = Activator.CreateInstance(prop.PropertyType);
                            InitField(scope, field);
                            prop.SetValue(block, field);
                        }
                    }
                    return block;
                }
            }
            return null;
        }

        /// <summary>
        /// Creates a dynamic region.
        /// </summary>
        /// <typeparam name="TValue">The value type</typeparam>
        /// <param name="type">The content type</param>
        /// <param name="regionId">The region id</param>
        /// <returns>The region value</returns>
        public TValue CreateRegion<TValue>(Models.ContentType contentType, string regionId)
        {
            using (var scope = _services.CreateScope())
            {
                var region = contentType.Regions.SingleOrDefault(r => r.Id == regionId);

                if (region != null)
                    return (TValue)CreateRegion(scope, typeof(TValue), region);
                return default(TValue);
            }
        }


        /// <summary>
        /// Gets the enumerator for the given region collection.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="model">The model</param>
        /// <param name="regionId">The region id</param>
        /// <returns>The enumerator</returns>
        public IEnumerable GetEnumerable<T>(T model, string regionId) where T : Models.Content
        {
            object value = null;

            if (model is Models.IDynamicModel)
            {
                value = ((IDictionary<string, object>)((Models.IDynamicModel)(object)model).Regions)[regionId];
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
        public object GetRegion<T>(T model, string regionId) where T : Models.Content
        {
            if (model is Models.IDynamicModel)
            {
                return ((IDictionary<string, object>)((Models.IDynamicModel)(object)model).Regions)[regionId];
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
        public bool HasRegion<T>(T model, string regionId) where T : Models.Content
        {
            if (model is Models.IDynamicModel)
            {
                return ((IDictionary<string, object>)((Models.IDynamicModel)(object)model).Regions).ContainsKey(regionId);
            }
            else
            {
                return model.GetType().GetProperty(regionId, App.PropertyBindings) != null;
            }
        }

        /// <summary>
        /// Loads the given data into a new model.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="content">The content entity</param>
        /// <param name="type">The content type</param>
        /// <returns>The page model</returns>
        public T Transform<T>(TContent content, Models.ContentType type, Action<TContent, T> process = null)
            where T : Models.Content, TModelBase
        {
            using (var scope = _services.CreateScope())
            {
                if (type != null)
                {
                    var modelType = typeof(T);

                    if (!typeof(Models.IDynamicModel).IsAssignableFrom(modelType) && !typeof(Models.IContentInfo).IsAssignableFrom(modelType))
                    {
                        modelType = Type.GetType(type.CLRType);

                        if (modelType != typeof(T) && !typeof(T).IsAssignableFrom(modelType))
                            return null;
                    }

                    // Create an initialized model
                    var model = Create<T>(type);

                    // Map basic fields
                    _mapper.Map<TContent, TModelBase>(content, model);

                    if (model is Models.RoutedContent)
                    {
                        var routeModel = (Models.RoutedContent)(object)model;

                        // Map route (if available)
                        if (string.IsNullOrWhiteSpace(routeModel.Route) && type.Routes.Count > 0)
                            routeModel.Route = type.Routes.First();
                    }

                    // Map regions
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
                                            SetSimpleValue(scope, model, regionKey, field);
                                            break;
                                        }
                                        else
                                        {
                                            SetComplexValue(scope, model, regionKey, fieldDef.Id, field);
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
                                            AddSimpleValue(scope, model, regionKey, field);
                                    }
                                    else
                                    {
                                        AddComplexValue(scope, model, type, regionKey, fields.Where(f => f.SortOrder == sortOrder).ToList());
                                    }
                                    sortOrder++;
                                }
                            }
                        }
                    }
                    process?.Invoke(content, model);

                    return model;
                }
            }
            return null;
        }

        /// <summary>
        /// Transforms the given model into content data.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="type">The conten type</param>
        /// <param name="dest">The optional dest object</param>
        /// <returns>The content data</returns>
        public TContent Transform<T>(T model, Models.ContentType type, TContent dest = null)
            where T : Models.Content, TModelBase
        {
            var content = dest == null ? Activator.CreateInstance<TContent>() : dest;

            // Map id
            if (model.Id != Guid.Empty)
            {
                content.Id = model.Id;
            }
            else
            {
                content.Id = model.Id = Guid.NewGuid();
            }
            content.Created = DateTime.Now;

            // Map basic fields
            _mapper.Map<TModelBase, TContent>(model, content);

            // Map regions
            var currentRegions = type.Regions.Select(r => r.Id).ToArray();

            foreach (var regionKey in currentRegions)
            {
                // Check that the region exists in the current model
                if (HasRegion(model, regionKey))
                {
                    var regionType = type.Regions.Single(r => r.Id == regionKey);

                    if (!regionType.Collection)
                    {
                        MapRegion(model, content, GetRegion(model, regionKey), regionType, regionKey);
                    }
                    else
                    {
                        var items = new List<Guid>();
                        var sortOrder = 0;
                        foreach (var region in GetEnumerable(model, regionKey))
                        {
                            var fields = MapRegion(model, content, region, regionType, regionKey, sortOrder++);

                            if (fields.Count > 0)
                                items.AddRange(fields);
                        }
                        // Now delete removed collection items
                        var removedFields = content.Fields
                            .Where(f => f.RegionId == regionKey && !items.Contains(f.Id))
                            .ToList();
                        foreach (var removed in removedFields)
                            content.Fields.Remove(removed);
                    }
                }
            }
            return content;
        }

        /// <summary>
        /// Transforms the given block data into block models.
        /// </summary>
        /// <param name="blocks">The data</param>
        /// <returns>The transformed blocks</returns>
        public IList<Extend.Block> TransformBlocks<T>(IEnumerable<T> blocks) where T : IContentBlock
        {
            using (var scope = _services.CreateScope())
            {
                var models = new List<Extend.Block>();

                foreach (var contentBlock in blocks)
                {
                    var block = contentBlock.Block;
                    var blockType = App.Blocks.GetByType(block.CLRType);

                    if (blockType != null)
                    {
                        var model = (Extend.Block)Activator.CreateInstance(blockType.Type);
                        model.Id = block.Id;

                        foreach (var field in block.Fields)
                        {
                            var prop = model.GetType().GetProperty(field.FieldId, App.PropertyBindings);

                            if (prop != null)
                            {
                                var type = App.Fields.GetByType(field.CLRType);
                                var val = (Extend.IField)App.DeserializeObject(field.Value, type.Type);

                                if (val != null)
                                {
                                    var init = val.GetType().GetMethod("Init");

                                    if (init != null)
                                    {
                                        var param = new List<object>();

                                        foreach (var p in init.GetParameters())
                                        {
                                            param.Add(scope.ServiceProvider.GetService(p.ParameterType));
                                        }

                                        // Check for async
                                        if (typeof(Task).IsAssignableFrom(init.ReturnType))
                                        {
                                            Task.Run(async () => await (Task)init.Invoke(val, param.ToArray())).Wait();
                                        }
                                        else
                                        {
                                            init.Invoke(val, param.ToArray());
                                        }
                                    }
                                }
                                prop.SetValue(model, val);
                            }
                        }

                        if (contentBlock.ParentId.HasValue)
                        {
                            var parent = blocks.FirstOrDefault(m => m.Id == contentBlock.ParentId.Value);

                            if (parent != null)
                            {
                                ((Extend.BlockGroup)models.First(m => m.Id == parent.BlockId))
                                    .Items.Add(model);
                            }
                            else throw new Exception("COULDN'T FIND PARENT");
                        }
                        else 
                        {
                            models.Add(model);
                        }
                    }
                }
                return models;
            }
        }        

        /// <summary>
        /// Transforms the given block data into block models.
        /// </summary>
        /// <param name="blocks">The data</param>
        /// <returns>The transformed blocks</returns>
        public IList<Extend.Block> TransformBlocks(IEnumerable<Block> blocks)
        {
            using (var scope = _services.CreateScope())
            {
                var models = new List<Extend.Block>();

                foreach (var block in blocks)
                {
                    var blockType = App.Blocks.GetByType(block.CLRType);

                    if (blockType != null)
                    {
                        var model = (Extend.Block)Activator.CreateInstance(blockType.Type);
                        model.Id = block.Id;

                        foreach (var field in block.Fields)
                        {
                            var prop = model.GetType().GetProperty(field.FieldId, App.PropertyBindings);

                            if (prop != null)
                            {
                                var type = App.Fields.GetByType(field.CLRType);
                                var val = (Extend.IField)App.DeserializeObject(field.Value, type.Type);

                                if (val != null)
                                {
                                    var init = val.GetType().GetMethod("Init");

                                    if (init != null)
                                    {
                                        var param = new List<object>();

                                        foreach (var p in init.GetParameters())
                                        {
                                            param.Add(scope.ServiceProvider.GetService(p.ParameterType));
                                        }

                                        // Check for async
                                        if (typeof(Task).IsAssignableFrom(init.ReturnType))
                                        {
                                            Task.Run(async () => await (Task)init.Invoke(val, param.ToArray())).Wait();
                                        }
                                        else
                                        {
                                            init.Invoke(val, param.ToArray());
                                        }
                                    }
                                }
                                prop.SetValue(model, val);
                            }
                        }
                        models.Add(model);
                    }
                }
                return models;
            }
        }

        /// <summary>
        /// Transforms the given blocks to the internal data model.
        /// </summary>
        /// <param name="models">The blocks</param>
        /// <returns>The data model</returns>
        public IList<T> TransformBlocks<T>(IList<Extend.Block> models) where T : IContentBlock
        {
            var blocks = new List<T>();

            if (models != null)
            {
                for (var n = 0; n < models.Count; n++)
                {
                    var type = App.Blocks.GetByType(models[n].GetType().FullName);

                    if (type != null)
                    {
                        var contentBlock = Activator.CreateInstance<T>();
                        contentBlock.Id = Guid.NewGuid();
                        contentBlock.SortOrder = n;

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
                        contentBlock.Block = block;
                        blocks.Add(contentBlock);

                        if (typeof(Extend.BlockGroup).IsAssignableFrom(models[n].GetType()))
                        {
                            var blockItems = TransformBlocks<T>(((Extend.BlockGroup)models[n]).Items);

                            if (blockItems.Count() > 0)
                            {
                                foreach (var item in blockItems)
                                {
                                    item.ParentId = contentBlock.Id;
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
        /// Transforms the given blocks to the internal data model.
        /// </summary>
        /// <param name="models">The blocks</param>
        /// <returns>The data model</returns>
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

        /// <summary>
        /// Maps a region to the given data entity.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="model">The model</param>
        /// <param name="content">The content entity</param>
        /// <param name="region">The region to map</param>
        /// <param name="regionType">The region type</param>
        /// <param name="regionId">The region id</param>
        /// <param name="sortOrder">The optional sort order</param>
        public IList<Guid> MapRegion<T>(T model, TContent content, object region, Models.RegionType regionType, string regionId, int sortOrder = 0) where T : Models.Content
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

                        // Update field info & value
                        field.CLRType = fieldType.TypeName;
                        field.SortOrder = sortOrder;
                        field.Value = App.SerializeObject(fieldValue, fieldType.Type);

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
        private void SetSimpleValue<T>(IServiceScope scope, T model, string regionId, TField field) where T : Models.Content
        {
            if (model is Models.IDynamicModel)
            {
                ((IDictionary<string, object>)((Models.IDynamicModel)(object)model).Regions)[regionId] =
                    DeserializeValue(scope, field);
            }
            else
            {
                model.GetType().GetProperty(regionId, App.PropertyBindings).SetValue(model,
                    DeserializeValue(scope, field));
            }
        }

        /// <summary>
        /// Adds a simple single field value to a collection region.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="model">The model</param>
        /// <param name="regionId">The region id</param>
        /// <param name="field">The field</param>
        private void AddSimpleValue<T>(IServiceScope scope, T model, string regionId, TField field) where T : Models.Content
        {
            if (model is Models.IDynamicModel)
            {
                ((IList)((IDictionary<string, object>)((Models.IDynamicModel)(object)model).Regions)[regionId]).Add(
                    DeserializeValue(scope, field));
            }
            else
            {
                ((IList)model.GetType().GetProperty(regionId, App.PropertyBindings).GetValue(model)).Add(
                    DeserializeValue(scope, field));
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
        private void SetComplexValue<T>(IServiceScope scope, T model, string regionId, string fieldId, TField field) where T : Models.Content
        {
            if (model is Models.IDynamicModel)
            {
                ((IDictionary<string, object>)((IDictionary<string, object>)((Models.IDynamicModel)(object)model).Regions)[regionId])[fieldId] =
                    DeserializeValue(scope, field);
            }
            else
            {
                var obj = model.GetType().GetProperty(regionId, App.PropertyBindings).GetValue(model);
                if (obj != null)
                    obj.GetType().GetProperty(fieldId, App.PropertyBindings).SetValue(obj,
                        DeserializeValue(scope, field));
            }
        }

        /// <summary>
        /// Adds a complex region to a collection region.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="model">The model</param>
        /// <param name="regionId">The region id</param>
        /// <param name="fields">The field</param>
        private void AddComplexValue<T>(IServiceScope scope, T model, Models.ContentType contentType, string regionId, IList<TField> fields) where T : Models.Content
        {
            if (fields.Count > 0)
            {
                if (model is Models.IDynamicModel)
                {
                    var list = (IList)((IDictionary<string, object>)((Models.IDynamicModel)(object)model).Regions)[regionId];
                    var obj = CreateDynamicRegion(contentType, regionId);

                    foreach (var field in fields)
                    {
                        if (((IDictionary<string, object>)obj).ContainsKey(field.FieldId))
                        {
                            ((IDictionary<string, object>)obj)[field.FieldId] =
                                DeserializeValue(scope, field);
                        }
                    }
                    list.Add(obj);

                }
                else
                {
                    var list = (IList)model.GetType().GetProperty(regionId, App.PropertyBindings).GetValue(model);
                    var obj = Activator.CreateInstance(list.GetType().GenericTypeArguments.First());

                    foreach (var field in fields)
                    {
                        var prop = obj.GetType().GetProperty(field.FieldId, App.PropertyBindings);
                        if (prop != null)
                        {
                            prop.SetValue(obj, DeserializeValue(scope, field));
                        }
                    }
                    list.Add(obj);
                }
            }
        }

        /// <summary>
        /// Deserializes the given field value.
        /// </summary>
        /// <param name="field">The page field</param>
        /// <returns>The value</returns>
        private object DeserializeValue(IServiceScope scope, TField field)
        {
            var type = App.Fields.GetByType(field.CLRType);

            if (type != null)
            {
                var val = (Extend.IField)App.DeserializeObject(field.Value, type.Type);
                if (val != null)
                {
                    var init = val.GetType().GetMethod("Init");

                    if (init != null)
                    {
                        var param = new List<object>();

                        foreach (var p in init.GetParameters())
                        {
                            param.Add(scope.ServiceProvider.GetService(p.ParameterType));
                        }

                        // Check for async
                        if (typeof(Task).IsAssignableFrom(init.ReturnType))
                        {
                            Task.Run(async () => await (Task)init.Invoke(val, param.ToArray())).Wait();
                        }
                        else
                        {
                            init.Invoke(val, param.ToArray());
                        }
                    }
                }
                return val;
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
                return region.GetType().GetProperty(fieldId, App.PropertyBindings).GetValue(region);
            }
        }

        /// <summary>
        /// Creates a dynamic region value.
        /// </summary>
        /// <param name="region">The region type</param>
        /// <returns>The created value</returns>
        private object CreateDynamicRegion(IServiceScope scope, RegionType region)
        {
            if (region.Fields.Count == 1)
            {
                var type = App.Fields.GetByShorthand(region.Fields[0].Type);
                if (type == null)
                    type = App.Fields.GetByType(region.Fields[0].Type);

                if (type != null)
                    return InitField(scope, Activator.CreateInstance(type.Type));
            }
            else
            {
                var reg = new ExpandoObject();

                foreach (var field in region.Fields)
                {
                    var type = GetFieldType(field);

                    if (type != null)
                        ((IDictionary<string, object>)reg).Add(field.Id, InitField(scope, Activator.CreateInstance(type.Type)));
                }
                return reg;
            }
            return null;
        }

        /// <summary>
        /// Creates a region value.
        /// </summary>
        /// <param name="regionType">The region type</param>
        /// <param name="region">The region</param>
        /// <returns>The created value</returns>
        private object CreateRegion(IServiceScope scope, Type regionType, RegionType region)
        {
            if (region.Fields.Count == 1)
            {
                return CreateField(scope, region.Fields[0], regionType);
            }
            else
            {
                var reg = Activator.CreateInstance(regionType);
                var type = reg.GetType();

                foreach (var field in region.Fields)
                {
                    var fieldType = GetFieldType(field);

                    if (type != null)
                    {
                        var prop = type.GetProperty(field.Id, App.PropertyBindings);

                        if (prop != null && fieldType.Type == prop.PropertyType)
                        {
                            prop.SetValue(reg, InitField(scope, Activator.CreateInstance(fieldType.Type)));
                        }
                    }
                }
                return reg;
            }
        }

        /// <summary>
        /// Gets the registered field type.
        /// </summary>
        /// <param name="field">The field</param>
        /// <returns>The type, null if not found</returns>
        private Runtime.AppField GetFieldType(FieldType field)
        {
            var type = App.Fields.GetByShorthand(field.Type);
            if (type == null)
                type = App.Fields.GetByType(field.Type);
            return type;
        }

        /// <summary>
        /// Creates a field of the given type.
        /// </summary>
        /// <param name="field">The field type</param>
        /// <param name="expectedType">The expected type</param>
        /// <returns></returns>
        private object CreateField(IServiceScope scope, FieldType field, Type expectedType = null)
        {
            var type = GetFieldType(field);

            if (type != null && (expectedType == null || type.Type == expectedType))
            {
                return InitField(scope, Activator.CreateInstance(type.Type));
            }
            return null;
        }

        private object InitField(IServiceScope scope, object field)
        {
            var init = field.GetType().GetMethod("Init");

            if (init != null)
            {
                var param = new List<object>();

                foreach (var p in init.GetParameters())
                {
                    param.Add(scope.ServiceProvider.GetService(p.ParameterType));
                }

                // Check for async
                if (typeof(Task).IsAssignableFrom(init.ReturnType))
                {
                    Task.Run(async () => await (Task)init.Invoke(field, param.ToArray())).Wait();
                }
                else
                {
                    init.Invoke(field, param.ToArray());
                }
            }
            return field;
        }
    }
}
