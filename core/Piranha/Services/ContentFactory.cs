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
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Piranha.Extend;
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
        public Task<T> CreateAsync<T>(ContentTypeBase type) where T : ContentBase
        {
            if (typeof(IDynamicContent).IsAssignableFrom(typeof(T)))
            {
                return CreateDynamicModelAsync<T>(type);
            }
            return CreateModelAsync<T>(type);
        }

        /// <summary>
        /// Creates a new dynamic region.
        /// </summary>
        /// <param name="type">The content type</param>
        /// <param name="regionId">The region id</param>
        /// <param name="managerInit">If manager initialization should be performed</param>
        /// <returns>The new region value</returns>
        public Task<object> CreateDynamicRegionAsync(ContentTypeBase type, string regionId, bool managerInit = false)
        {
            using (var scope = _services.CreateScope())
            {
                var region = type.Regions.FirstOrDefault(r => r.Id == regionId);

                if (region != null)
                {
                    return CreateDynamicRegionAsync(scope, region, true, managerInit);
                }
                return null;
            }
        }

        /// <summary>
        /// Creates and initializes a new block of the specified type.
        /// </summary>
        /// <param name="typeName">The type name</param>
        /// <param name="managerInit">If manager initialization should be performed</param>
        /// <returns>The new block</returns>
        public async Task<object> CreateBlockAsync(string typeName, bool managerInit = false)
        {
            var blockType = App.Blocks.GetByType(typeName);

            if (blockType != null)
            {
                using (var scope = _services.CreateScope())
                {
                    var block = (Extend.Block)Activator.CreateInstance(blockType.Type);
                    block.Type = typeName;

                    foreach (var prop in blockType.Type.GetProperties(App.PropertyBindings))
                    {
                        if (typeof(Extend.IField).IsAssignableFrom(prop.PropertyType))
                        {
                            var field = Activator.CreateInstance(prop.PropertyType);
                            await InitFieldAsync(scope, field, managerInit).ConfigureAwait(false);
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
        private async Task<T> CreateDynamicModelAsync<T>(ContentTypeBase type) where T : ContentBase
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
                        region = await CreateDynamicRegionAsync(scope, regionType).ConfigureAwait(false);
                    }
                    else
                    {
                        // Create a region item without initialization for type reference
                        var listObject = await CreateDynamicRegionAsync(scope, regionType, false).ConfigureAwait(false);

                        if (listObject != null)
                        {
                            // Create the region list
                            region = Activator.CreateInstance(typeof(RegionList<>).MakeGenericType(listObject.GetType()));
                            ((IRegionList)region).Model = (IDynamicContent)model;
                            ((IRegionList)region).TypeId = type.Id;
                            ((IRegionList)region).RegionId = regionType.Id;
                        }
                    }

                    if (region != null)
                    {
                        ((IDictionary<string, object>)((IDynamicContent)model).Regions).Add(regionType.Id, region);
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
        private async Task<T> CreateModelAsync<T>(ContentTypeBase type) where T : ContentBase
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
                        region = await CreateRegionAsync(scope, model, modelType, regionType).ConfigureAwait(false);
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
        public Task<T> InitDynamicAsync<T>(T model, ContentTypeBase type) where T : IDynamicContent
        {
            return InitDynamicAsync<T>(model, type, false);
        }

        /// <summary>
        /// Initializes the given dynamic model for the manager.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="type">The content type</param>
        /// <typeparam name="T">The model type</typeparam>
        /// <returns>The initialized model</returns>
        public Task<T> InitDynamicManagerAsync<T>(T model, ContentTypeBase type) where T : IDynamicContent
        {
            return InitDynamicAsync<T>(model, type, true);
        }

        /// <summary>
        /// Initializes the given dynamic model.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="type">The content type</param>
        /// <param name="managerInit">If this is initialization used by the manager</param>
        /// <typeparam name="T">The model type</typeparam>
        /// <returns>The initialized model</returns>
        private async Task<T> InitDynamicAsync<T>(T model, ContentTypeBase type, bool managerInit) where T : IDynamicContent
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
                            await InitDynamicRegionAsync(scope, region, regionType, managerInit).ConfigureAwait(false);
                        }
                        else
                        {
                            // This region was a collection. Initialize all items
                            foreach (var item in (IList)region)
                            {
                                await InitDynamicRegionAsync(scope, item, regionType, managerInit).ConfigureAwait(false);
                            }
                        }
                    }
                }

                if (model is IBlockContent blockModel)
                {
                    foreach (var block in blockModel.Blocks)
                    {
                        await InitBlockAsync(scope, block, managerInit).ConfigureAwait(false);

                        if (block is BlockGroup blockGroup)
                        {
                            foreach (var child in blockGroup.Items)
                            {
                                await InitBlockAsync(scope, child, managerInit).ConfigureAwait(false);
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
        public Task<T> InitAsync<T>(T model, ContentTypeBase type) where T : ContentBase
        {
            return InitAsync<T>(model, type, false);
        }

        /// <summary>
        /// Initializes the given model for the manager.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="type">The content type</param>
        /// <typeparam name="T">The model type</typeparam>
        /// <returns>The initialized model</returns>
        public Task<T> InitManagerAsync<T>(T model, ContentTypeBase type) where T : ContentBase
        {
            return InitAsync<T>(model, type, true);
        }

        /// <summary>
        /// Initializes the given field.
        /// </summary>
        /// <param name="field">The field</param>
        /// <param name="managerInit">If this is initialization used by the manager</param>
        /// <returns>The initialized field</returns>
        public async Task<object> InitFieldAsync(object field, bool managerInit = false)
        {
            using (var scope = _services.CreateScope())
            {
                return await InitFieldAsync(scope, field, managerInit).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Initializes the given model.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="type">The content type</param>
        /// <param name="managerInit">If this is initialization used by the manager</param>
        /// <typeparam name="T">The model type</typeparam>
        /// <returns>The initialized model</returns>
        private async Task<T> InitAsync<T>(T model, ContentTypeBase type, bool managerInit) where T : ContentBase
        {
            if (model is IDynamicContent)
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
                            await InitRegionAsync(scope, region, regionType, managerInit).ConfigureAwait(false);
                        }
                        else
                        {
                            // This region was a collection. Initialize all items
                            foreach (var item in (IList)region)
                            {
                                await InitRegionAsync(scope, item, regionType, managerInit).ConfigureAwait(false);
                            }
                        }
                    }
                }

                if (!(model is IContentInfo) && model is IBlockContent blockModel)
                {
                    foreach (var block in blockModel.Blocks)
                    {
                        await InitBlockAsync(scope, block, managerInit).ConfigureAwait(false);

                        if (block is Extend.BlockGroup)
                        {
                            foreach (var child in ((Extend.BlockGroup)block).Items)
                            {
                                await InitBlockAsync(scope, child, managerInit).ConfigureAwait(false);
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
        /// <param name="managerInit">If this is initialization used by the manager</param>
        private async Task InitDynamicRegionAsync(IServiceScope scope, object region, ContentTypeRegion regionType, bool managerInit)
        {
            if (region != null)
            {
                if (regionType.Fields.Count == 1)
                {
                    // This region only has one field, that means
                    // the region is in fact a field.
                    await InitFieldAsync(scope, region, managerInit).ConfigureAwait(false);
                }
                else
                {
                    // Initialize all fields
                    foreach (var fieldType in regionType.Fields)
                    {
                        if (((IDictionary<string, object>)region).TryGetValue(fieldType.Id, out var field))
                        {
                            await InitFieldAsync(scope, field, managerInit).ConfigureAwait(false);
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
        /// <param name="managerInit">If this is initialization used by the manager</param>
        private async Task InitRegionAsync(IServiceScope scope, object region, ContentTypeRegion regionType, bool managerInit)
        {
            if (region != null)
            {
                if (regionType.Fields.Count == 1)
                {
                    // This region only has one field, that means
                    // the region is in fact a field.
                    await InitFieldAsync(scope, region, managerInit).ConfigureAwait(false);
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
                            await InitFieldAsync(scope, field, managerInit).ConfigureAwait(false);
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
        /// <param name="managerInit">If this is initialization used by the manager</param>
        private async Task InitBlockAsync(IServiceScope scope, Extend.Block block, bool managerInit)
        {
            if (block != null)
            {
                var properties = block.GetType().GetProperties(App.PropertyBindings);

                // Initialize all of the fields
                foreach (var property in properties)
                {
                    if (typeof(Extend.IField).IsAssignableFrom(property.PropertyType))
                    {
                        var field = property.GetValue(block);

                        if (field != null)
                        {
                            await InitFieldAsync(scope, field, managerInit).ConfigureAwait(false);
                        }
                    }
                }

                // Initialize the block
                var appBlock = App.Blocks.GetByType(block.GetType());
                if (appBlock != null)
                {
                    await appBlock.Init.InvokeAsync(block, scope, managerInit).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Creates a new dynamic region.
        /// </summary>
        /// <param name="scope">The current service scope</param>
        /// <param name="regionType">The region type</param>
        /// <param name="initFields">If fields should be initialized</param>
        /// <param name="managerInit">If manager init should be performed on the fields</param>
        /// <returns>The created region</returns>
        private async Task<object> CreateDynamicRegionAsync(IServiceScope scope, ContentTypeRegion regionType, bool initFields = true, bool managerInit = false)
        {
            if (regionType.Fields.Count == 1)
            {
                var field = CreateField(regionType.Fields[0]);
                if (field != null)
                {
                    if (initFields)
                    {
                        await InitFieldAsync(scope, field, managerInit).ConfigureAwait(false);
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
                            await InitFieldAsync(scope, field, managerInit).ConfigureAwait(false);
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
        /// <param name="model">The model to create the region for</param>
        /// <param name="modelType">The model type</param>
        /// <param name="regionType">The region type</param>
        /// <param name="initFields">If fields should be initialized</param>
        /// <returns>The created region</returns>
        private async Task<object> CreateRegionAsync(IServiceScope scope, object model, Type modelType, ContentTypeRegion regionType, bool initFields = true)
        {
            if (regionType.Fields.Count == 1)
            {
                var field = CreateField(regionType.Fields[0]);
                if (field != null)
                {
                    if (initFields)
                    {
                        await InitFieldAsync(scope, field, false).ConfigureAwait(false);
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
                                await InitFieldAsync(scope, field, false).ConfigureAwait(false);
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
        private object CreateField(ContentTypeField fieldType)
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
        /// <param name="managerInit">If this is initialization used by the manager</param>
        /// <returns>The initialized field</returns>
        private async Task<object> InitFieldAsync(IServiceScope scope, object field, bool managerInit)
        {
            var appField = App.Fields.GetByType(field.GetType());

            if (appField != null)
            {
                await appField.Init.InvokeAsync(field, scope, managerInit);
            }
            return field;
        }
    }
}