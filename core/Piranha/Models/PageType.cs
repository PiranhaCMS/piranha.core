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

namespace Piranha.Models
{
    [Serializable]
    public sealed class PageType : ContentTypeBase
    {
        /// <summary>
        /// Gets/sets if the page type should use the block editor
        /// for its main content. The default value is True.
        /// </summary>
        public bool UseBlocks { get; set; } = true;

        /// <summary>
        /// Gets/sets if this is an archive page or not.
        /// </summary>
        public bool IsArchive { get; set; }

        /// <summary>
        /// Gets/sets the allowed items types if this is an Archive
        /// Page. If the collection is empty all Post Types should
        /// be considered to be allowed.
        /// </summary>
        public IList<string> ArchiveItemTypes { get; set; } = new List<string>();

        /// <summary>
        /// Validates that the page type is correctly defined.
        /// </summary>
        public void Ensure() {
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
