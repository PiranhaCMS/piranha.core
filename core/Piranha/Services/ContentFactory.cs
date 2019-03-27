/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Piranha.Models;

namespace Piranha.Services
{
    /// <summary>
    /// The content factory is responsible for creating models and
    /// initializing them after they have been loaded.
    /// </summary>
    public class ContentFactory : IContentFactory
    {
        private readonly IServiceProvider _services;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="services">The current service provider</param>
        public ContentFactory(IServiceProvider services)
        {
            _services = services;
        }

        /// <summary>
        /// Creates and initializes a new content model.
        /// </summary>
        /// <param name="type">The content type</param>
        /// <typeparam name="T">The model type</typeparam>
        /// <returns>The new model</returns>
        public T Create<T>(ContentType type) where T : Content
        {
            if (typeof(IDynamicModel).IsAssignableFrom(typeof(T)))
            {
                return CreateDynamicModel<T>(type);
            }
            else
            {
                return CreateModel<T>(type);
            }
        }

        /// <summary>
        /// Creates a new dynamic region.
        /// </summary>
        /// <param name="type">The content type</param>
        /// <param name="regionId">The region id</param>
        /// <returns>The new region value</returns>
        public object CreateDynamicRegion(ContentType type, string regionId)
        {
            using (var scope = _services.CreateScope())
            {
                var region = type.Regions.FirstOrDefault(r => r.Id == regionId);

                if (region != null)
                {
                    return CreateDynamicRegion(scope, region);
                }
                return null;
            }
        }

        /// <summary>
        /// Creates and initializes a new block of the specified type.
        /// </summary>
        /// <param name="typeName">The type name</param>
        /// <returns>The new block</returns>
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
        /// Creates and initializes a new dynamic model.
        /// </summary>
        /// <param name="type">The content type</param>
        /// <typeparam name="T">The model type</typeparam>
        /// <returns>The new model</returns>
        private T CreateDynamicModel<T>(ContentType type) where T : Content
        {
            using (var scope = _services.CreateScope())
            {
                // Create a new instance of the specified type
                var model = Activator.CreateInstance<T>();

                model.TypeId = type.Id;

                foreach (var regionType in type.Regions)
                {
                    object region = null;

                    if (!regionType.Collection)
                    {
                        // Create and initialize the region
                        region = CreateDynamicRegion(scope, regionType);
                    }
                    else
                    {
                        // Create a region item without initialization for type reference
                        var listObject = CreateDynamicRegion(scope, regionType, false);

                        if (listObject != null)
                        {
                            // Create the region list
                            region = Activator.CreateInstance(typeof(RegionList<>).MakeGenericType(listObject.GetType()));
                            ((IRegionList)region).Model = (IDynamicModel)model;
                            ((IRegionList)region).TypeId = type.Id;
                            ((IRegionList)region).RegionId = regionType.Id;
                        }
                    }

                    if (region != null)
                    {
                        ((IDictionary<string, object>)((IDynamicModel)model).Regions).Add(regionType.Id, region);
                    }
                }
                return model;
            }
        }

        /// <summary>
        /// Creates and initializes a new dynamic model.
        /// </summary>
        /// <param name="type">The content type</param>
        /// <typeparam name="T">The model type</typeparam>
        /// <returns>The new model</returns>
        private T CreateModel<T>(ContentType type) where T : Content
        {
            using (var scope = _services.CreateScope())
            {
                var modelType = typeof(T);

                if (!typeof(IContentInfo).IsAssignableFrom(modelType))
                {
                    modelType = Type.GetType(type.CLRType);

                    if (modelType != typeof(T) && !typeof(T).IsAssignableFrom(modelType))
                    {
                        return null;
                    }
                }

                // Create a new instance of the specified type
                var model = (T)Activator.CreateInstance(modelType);

                model.TypeId = type.Id;

                foreach (var regionType in type.Regions)
                {
                    object region = null;

                    if (!regionType.Collection)
                    {
                        // Create and initialize the region
                        region = CreateRegion(scope, model, modelType, regionType);
                    }
                    else
                    {
                        var property = modelType.GetProperty(regionType.Id, App.PropertyBindings);

                        if (property != null)
                        {
                            region = Activator.CreateInstance(typeof(List<>).MakeGenericType(property.PropertyType.GetGenericArguments()[0]));
                        }
                    }

                    if (region != null)
                    {
                        modelType.SetPropertyValue(regionType.Id, model, region);
                    }
                }
                return model;
            }
        }

        /// <summary>
        /// Initializes the given dynamic model.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="type">The content type</param>
        /// <typeparam name="T">The model type</typeparam>
        /// <returns>The initialized model</returns>
        public T InitDynamic<T>(T model, ContentType type) where T : IDynamicModel
        {
            using (var scope = _services.CreateScope())
            {
                foreach (var regionType in type.Regions)
                {
                    // Try to get the region from the model
                    if (((IDictionary<string, object>)model.Regions).TryGetValue(regionType.Id, out var region))
                    {
                        if (!regionType.Collection)
                        {
                            // Initialize it
                            InitDynamicRegion(scope, region, regionType);
                        }
                        else
                        {
                            // This region was a collection. Initialize all items
                            foreach (var item in (IList)region)
                            {
                                InitDynamicRegion(scope, item, regionType);
                            }
                        }
                    }
                }

                if (model is IBlockModel)
                {
                    foreach (var block in ((IBlockModel)model).Blocks)
                    {
                        InitBlock(scope, block);


                        if (block is Extend.BlockGroup)
                        {
                            foreach (var child in ((Extend.BlockGroup)block).Items)
                            {
                                InitBlock(scope, child);
                            }
                        }
                    }
                }
            }
            return model;
        }

        /// <summary>
        /// Initializes the given model.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="type">The content type</param>
        /// <typeparam name="T">The model type</typeparam>
        /// <returns>The initialized model</returns>
        public T Init<T>(T model, ContentType type) where T : Content
        {
            if (model is IDynamicModel)
            {
                throw new ArgumentException("For dynamic models InitDynamic should be used.");
            }

            using (var scope = _services.CreateScope())
            {
                foreach (var regionType in type.Regions)
                {
                    // Try to get the region from the model
                    var region = model.GetType().GetPropertyValue(regionType.Id, model);

                    if (region != null)
                    {
                        if (!regionType.Collection)
                        {
                            // Initialize it
                            InitRegion(scope, region, regionType);
                        }
                        else
                        {
                            // This region was a collection. Initialize all items
                            foreach (var item in (IList)region)
                            {
                                InitRegion(scope, item, regionType);
                            }
                        }
                    }
                }

                if (!(model is IContentInfo) && model is IBlockModel)
                {
                    foreach (var block in ((IBlockModel)model).Blocks)
                    {
                        InitBlock(scope, block);

                        if (block is Extend.BlockGroup)
                        {
                            foreach (var child in ((Extend.BlockGroup)block).Items)
                            {
                                InitBlock(scope, child);
                            }
                        }
                    }
                }
            }
            return model;
        }

        /// <summary>
        /// Initializes all fields in the given dynamic region.
        /// </summary>
        /// <param name="scope">The current service scope</param>
        /// <param name="region">The region</param>
        /// <param name="regionType">The region type</param>
        private void InitDynamicRegion(IServiceScope scope, object region, RegionType regionType)
        {
            if (region != null)
            {
                if (regionType.Fields.Count == 1)
                {
                    // This region only has one field, that means
                    // the region is in fact a field.
                    InitField(scope, region);
                }
                else
                {
                    // Initialize all fields
                    foreach (var fieldType in regionType.Fields)
                    {
                        if (((IDictionary<string, object>)region).TryGetValue(fieldType.Id, out var field))
                        {
                            InitField(scope, field);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Initializes all fields in the given region.
        /// </summary>
        /// <param name="scope">The current service scope</param>
        /// <param name="region">The region</param>
        /// <param name="regionType">The region type</param>
        private void InitRegion(IServiceScope scope, object region, RegionType regionType)
        {
            if (region != null)
            {
                if (regionType.Fields.Count == 1)
                {
                    // This region only has one field, that means
                    // the region is in fact a field.
                    InitField(scope, region);
                }
                else
                {
                    var type = region.GetType();

                    // Initialize all fields
                    foreach (var fieldType in regionType.Fields)
                    {
                        var field = type.GetPropertyValue(fieldType.Id, region);

                        if (field != null)
                        {
                            InitField(scope, field);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Initializes all fields in the given block.
        /// </summary>
        /// <param name="scope">The current service scope</param>
        /// <param name="block">The block</param>
        /// <param name="regionType">The region type</param>
        private void InitBlock(IServiceScope scope, Extend.Block block)
        {
            if (block != null)
            {
                var type = block.GetType();
                var properties = block.GetType().GetProperties(App.PropertyBindings);

                foreach (var property in properties)
                {
                    if (typeof(Extend.IField).IsAssignableFrom(property.PropertyType))
                    {
                        var field = property.GetValue(block);

                        if (field != null)
                        {
                            InitField(scope, field);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new dynamic region.
        /// </summary>
        /// <param name="scope">The current service scope</param>
        /// <param name="regionType">The region type</param>
        /// <param name="initFields">If fields should be initialized</param>
        /// <returns>The created region</returns>
        private object CreateDynamicRegion(IServiceScope scope, RegionType regionType, bool initFields = true)
        {
            if (regionType.Fields.Count == 1)
            {
                var field = CreateField(regionType.Fields[0]);
                if (field != null)
                {
                    if (initFields)
                    {
                        InitField(scope, field);
                    }
                    return field;
                }
            }
            else
            {
                var reg = new ExpandoObject();

                foreach (var fieldType in regionType.Fields)
                {
                    var field = CreateField(fieldType);
                    if (field != null)
                    {
                        if (initFields)
                        {
                            InitField(scope, field);
                        }
                        ((IDictionary<string, object>)reg).Add(fieldType.Id, field);
                    }
                }
                return reg;
            }
            return null;
        }

        /// <summary>
        /// Creates a new region.
        /// </summary>
        /// <param name="scope">The current service scope</param>
        /// <param name="regionType">The region type</param>
        /// <param name="initFields">If fields should be initialized</param>
        /// <returns>The created region</returns>
        private object CreateRegion(IServiceScope scope, object model, Type modelType, RegionType regionType, bool initFields = true)
        {
            if (regionType.Fields.Count == 1)
            {
                var field = CreateField(regionType.Fields[0]);
                if (field != null)
                {
                    if (initFields)
                    {
                        InitField(scope, field);
                    }
                    return field;
                }
            }
            else
            {
                var property = modelType.GetProperty(regionType.Id, App.PropertyBindings);

                if (property != null)
                {
                    var reg = Activator.CreateInstance(property.PropertyType);

                    foreach (var fieldType in regionType.Fields)
                    {
                        var field = CreateField(fieldType);
                        if (field != null)
                        {
                            if (initFields)
                            {
                                InitField(scope, field);
                            }
                            reg.GetType().SetPropertyValue(fieldType.Id, reg, field);
                        }
                    }
                    return reg;
                }
            }
            return null;
        }

        /// <summary>
        /// Creates a new instance of the given field type.
        /// </summary>
        /// <param name="fieldType">The field type</param>
        /// <returns>The new instance</returns>
        protected object CreateField(FieldType fieldType)
        {
            var type = App.Fields.GetByType(fieldType.Type);

            if (type != null)
            {
                return Activator.CreateInstance(type.Type);
            }
            return null;
        }

        /// <summary>
        /// Initializes the given field.
        /// </summary>
        /// <param name="scope">The current service scope</param>
        /// <param name="field">The field</param>
        /// <returns>The initialized field</returns>
        protected object InitField(IServiceScope scope, object field)
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
                    Task.Run(async () => await ((Task)init.Invoke(field, param.ToArray())).ConfigureAwait(false)).Wait();
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