/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using Piranha.Extend;

namespace Piranha.Builder.Attribute
{
    public class BlockTypeBuilder : ContentTypeBuilder<BlockTypeBuilder, BlockType>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="logFactory">The optional log factory</param>
        public BlockTypeBuilder(IApi api, ILoggerFactory logFactory = null) : base(api, logFactory) { }

        /// <summary>
        /// Builds the page types.
        /// </summary>
        public override void Build() {
            foreach (var type in types) {
                var blockType = GetContentType(type);

                if (blockType != null)
                    api.BlockTypes.Save(blockType);
            }
            // Tell the app to reload the page types
            App.ReloadBlockTypes(api);
        }

        #region Private methods
        /// <summary>
        /// Gets the possible page type for the given type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The page type</returns>
        protected override BlockType GetContentType(Type type) {
            var attr = type.GetTypeInfo().GetCustomAttribute<BlockTypeAttribute>();

            if (attr != null) {
                logger?.LogInformation($"Importing BlockType '{type.Name}'.");

                if (!string.IsNullOrEmpty(attr.Id) && !string.IsNullOrEmpty(attr.Title)) {
                    var blockType = new BlockType() {
                        Id = attr.Id,
                        Title = attr.Title,
                        View = attr.View
                    };

                    foreach (var prop in type.GetTypeInfo().GetProperties(App.PropertyBindings)) {
                        var regionType = GetRegionType(prop);

                        if (regionType != null)
                            blockType.Regions.Add(regionType);
                    }
                    return blockType;
                } else {
                    logger?.LogError($"Id and/or Title is missing for BlockType '{type.Name}'.");
                }
            } 
            return null;
        }
        #endregion
    }
}
