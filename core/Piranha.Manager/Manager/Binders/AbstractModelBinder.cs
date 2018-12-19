/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Piranha.Areas.Manager.Models;

namespace Piranha.Manager.Binders
{
    internal sealed class AbstractModelBinder : IModelBinder
    {
        /// <summary>
        /// The meta data provider from the current binding context.
        /// </summary>
        private readonly IModelMetadataProvider provider;

        /// <summary>
        /// The available binders.
        /// </summary>
        private readonly Dictionary<string, AbstractBinderType> binders;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="provider">The current meta data provider</param>
        /// <param name="binders">The available binders</param>
        public AbstractModelBinder(IModelMetadataProvider provider, Dictionary<string, AbstractBinderType> binders) {
            this.provider = provider;
            this.binders = binders;
        }

        /// <summary>
        /// Binds the current model from the context.
        /// </summary>
        /// <param name="bc">The binding context</param>
        /// <returns>An asynchronous task</returns>
        public async Task BindModelAsync(ModelBindingContext bc) {
            var result = ModelBindingResult.Failed();
            var typeName = "";

            // Get the requested abstract type
            if (bc.ModelType == typeof(PageEditRegionBase))
                typeName = bc.ValueProvider.GetValue(bc.ModelName + ".CLRType").FirstValue;
            else if (bc.ModelType == typeof(Extend.IField))
                typeName = bc.ValueProvider.GetValue(bc.ModelName.Replace(".Value", "") + ".CLRType").FirstValue;
            else if (bc.ModelType == typeof(Extend.Block))
                typeName = bc.ValueProvider.GetValue(bc.ModelName.Replace(".Value", "") + ".CLRType").FirstValue;

            if (!String.IsNullOrEmpty(typeName)) {
                try {
                    if (binders.ContainsKey(typeName)) {
                        // Get the binder for the abstract type
                        var item = binders[typeName];
                        var metadata = provider.GetMetadataForType(item.Type);

                        // Let the default binders take care of it once
                        // that the real type has been discovered.
                        ModelBindingResult scoped;
                        using (bc.EnterNestedScope(
                            metadata,
                            bc.FieldName,
                            bc.ModelName,
                            model: null)) {
                            await item.Binder.BindModelAsync(bc);
                            scoped = bc.Result;
                        }
                        result = scoped;
                    } 
                } catch { }
            }
            bc.Result = result;
        }
    }
}
