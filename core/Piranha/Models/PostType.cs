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
using System.Linq;

namespace Piranha.Models
{
    [Serializable]
    public sealed class PostType : ContentTypeBase
    {
        /// <summary>
        /// Gets/sets if the post type should use the block editor
        /// for its main content. The default value is true.
        /// </summary>
        public bool UseBlocks { get; set; } = true;

        /// <summary>
        /// Gets/sets if the post type should use the default primary
        /// image. The default value is true.
        /// </summary>
        public bool UsePrimaryImage { get; set; } = true;

        /// <summary>
        /// Validates that the page type is correctly defined.
        /// </summary>
        public void Ensure()
        {
            if (Regions.Select(r => r.Id).Distinct().Count() != Regions.Count)
            {
                throw new InvalidOperationException($"Region Id not unique for page type {Id}");
            }

            foreach (var region in Regions)
            {
                region.Title = region.Title ?? region.Id;

                if (region.Fields.Select(f => f.Id).Distinct().Count() != region.Fields.Count)
                {
                    throw new InvalidOperationException($"Field Id not unique for page type {Id}");
                }

                foreach (var field in region.Fields)
                {
                    field.Id = field.Id ?? "Default";
                    field.Title = field.Title ?? field.Id;
                }
            }
        }
    }
}
