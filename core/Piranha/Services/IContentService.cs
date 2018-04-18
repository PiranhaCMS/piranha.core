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
using System.Threading.Tasks;

namespace Piranha.Services
{
    public interface IContentService<TContent, TField, TModelBase>
        where TContent : Data.Content<TField> 
        where TField : Data.ContentField
        where TModelBase : Models.Content
    {
        /// <summary>
        /// Creates a new content model of the given type.
        /// </summary>
        /// <param name="contentType">The content type</param>
        /// <returns>The model</returns>
        T Create<T>(Models.ContentType contentType) where T : Models.Content;

        /// <summary>
        /// Creates a new region.
        /// </summary>
        /// <param name="typeId">The content type id</param>
        /// <param name="regionId">The region id</param>
        /// <returns>The new region value</returns>
        object CreateDynamicRegion(Models.ContentType contentType, string regionId);

        /// <summary>
        /// Creates a dynamic region.
        /// </summary>
        /// <typeparam name="TValue">The value type</typeparam>
        /// <param name="type">The content type</param>
        /// <param name="regionId">The region id</param>
        /// <returns>The region value</returns>
        T CreateRegion<T>(Models.ContentType contentType, string regionId);

        /// <summary>
        /// Transforms the given data into a new model.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="content">The content entity</param>
        /// <param name="type">The content type</param>
        /// <returns>The page model</returns>
        T Transform<T>(TContent content, Models.ContentType type, Action<TContent, T> process = null) 
            where T : Models.Content, TModelBase;

        /// <summary>
        /// Transforms the given model into content data.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="type">The conten type</param>
        /// <param name="dest">The optional dest object</param>
        /// <returns>The content data</returns>
        TContent Transform<T>(T model, Models.ContentType type, TContent dest = null)
            where T : Models.Content, TModelBase;            
    }
}
