/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Piranha.Models
{
    /// <summary>
    /// Factory for creating templated content.
    /// </summary>
    internal class ContentFactory : IDisposable
    {
        /// <summary>
        /// The current content types.
        /// </summary>
        private readonly IEnumerable<ContentType> _types;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="types"></param>
        public ContentFactory(IEnumerable<ContentType> types)
        {
            _types = types;
        }

        /// <summary>
        /// Creates a new region.
        /// </summary>
        /// <param name="typeId">The content type id</param>
        /// <param name="regionId">The region id</param>
        /// <returns>The new region value</returns>
        public object CreateDynamicRegion(string typeId, string regionId)
        {
            var contentType = _types
                .SingleOrDefault(t => t.Id == typeId);

            if (contentType != null)
            {
                var region = contentType.Regions.SingleOrDefault(r => r.Id == regionId);

                if (region != null)
                    return CreateDynamicRegion(region);
            }
            return null;
        }

        /// <summary>
        /// Disposes the factory.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Creates a dynamic region value.
        /// </summary>
        /// <param name="region">The region type</param>
        /// <returns>The created value</returns>
        private object CreateDynamicRegion(RegionType region)
        {
            if (region.Fields.Count == 1)
            {
                var type = App.Fields.GetByShorthand(region.Fields[0].Type);
                if (type == null)
                    type = App.Fields.GetByType(region.Fields[0].Type);

                if (type != null)
                    return Activator.CreateInstance(type.Type);
            }
            else
            {
                var reg = new ExpandoObject();

                foreach (var field in region.Fields)
                {
                    var type = GetFieldType(field);

                    if (type != null)
                        ((IDictionary<string, object>)reg).Add(field.Id, Activator.CreateInstance(type.Type));
                }
                return reg;
            }
            return null;
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
    }
}
