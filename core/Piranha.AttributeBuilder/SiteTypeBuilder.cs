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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Piranha.Models;

namespace Piranha.AttributeBuilder
{
    /// <summary>
    /// Class for building and importing site types.
    /// </summary>
    public class SiteTypeBuilder : ContentTypeBuilder<SiteTypeBuilder, SiteType>
    {
        private readonly IApi _api;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public SiteTypeBuilder(IApi api)
        {
            _api = api;
        }

        /// <summary>
        /// Builds the site types.
        /// </summary>
        public override async Task<SiteTypeBuilder> BuildAsync()
        {
            foreach (var siteType in _types.Select(GetContentType).Where(siteType => siteType != null))
            {
                siteType.Ensure();
                await _api.SiteTypes.SaveAsync(siteType);
            }

            return this;
        }

        /// <summary>
        /// Deletes all site types in the database that doesn't
        /// exist in the database,
        /// </summary>
        /// <returns>The builder</returns>
        public SiteTypeBuilder DeleteOrphans()
        {
            return DeleteOrphansAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Deletes all site types in the database that doesn't
        /// exist in the import,
        /// </summary>
        /// <returns>The builder</returns>
        public async Task<SiteTypeBuilder> DeleteOrphansAsync()
        {
            var importTypes = _types.Select(GetContentType).Where(importType => importType != null).ToList();

            // Get all site types added for import.

            // Get all previously imported page types.
            var orphans = (await _api.SiteTypes.GetAllAsync()).Where(siteType => importTypes.All(t => t.Id != siteType.Id)).ToList();

            // Delete all orphans.
            foreach (var siteType in orphans)
            {
                await _api.SiteTypes.DeleteAsync(siteType);
            }
            return this;
        }

        #region Private methods
        /// <summary>
        /// Gets the possible page type for the given type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The page type</returns>
        protected override SiteType GetContentType(Type type)
        {
            var attr = type.GetTypeInfo().GetCustomAttribute<SiteTypeAttribute>();

            if (attr != null)
            {
                if (string.IsNullOrWhiteSpace(attr.Id))
                    attr.Id = type.Name;

                if (!string.IsNullOrEmpty(attr.Id) && !string.IsNullOrEmpty(attr.Title))
                {
                    var siteType = new SiteType
                    {
                        Id = attr.Id,
                        CLRType = type.GetTypeInfo().AssemblyQualifiedName,
                        Title = attr.Title
                    };

                    var regionTypes = new List<Tuple<int?, RegionType>>();

                    foreach (var prop in type.GetProperties(App.PropertyBindings))
                    {
                        var regionType = GetRegionType(prop);

                        if (regionType != null)
                        {
                            regionTypes.Add(regionType);
                        }
                    }
                    regionTypes = regionTypes.OrderBy(t => t.Item1).ToList();

                    // First add sorted regions
                    foreach (var regionType in regionTypes.Where(t => t.Item1.HasValue))
                    {
                        siteType.Regions.Add(regionType.Item2);
                    }
                    // Then add the unsorted regions
                    foreach (var regionType in regionTypes.Where(t => !t.Item1.HasValue))
                    {
                        siteType.Regions.Add(regionType.Item2);
                    }
                    return siteType;
                }
            }
            else
            {
                throw new ArgumentException($"Title is mandatory in SiteTypeAttribute. No title provided for {type.Name}");
            }
            return null;
        }
        #endregion
    }
}
