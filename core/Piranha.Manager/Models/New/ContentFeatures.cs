/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Manager.Models
{
    public sealed class ContentFeatures
    {
        /// <summary>
        /// Gets/sets if alt title should be used.
        /// </summary>
        public bool UseAltTitle { get; set; }

        /// <summary>
        /// Gets/sets if blocks should be used.
        /// </summary>
        public bool UseBlocks { get; set; }

        /// <summary>
        /// Gets/sets if the content type should be
        /// categorized.
        /// </summary>
        public bool UseCategory { get; set; }

        /// <summary>
        /// Gets/sets if comments should be enabled.
        /// </summary>
        public bool UseComments { get; set; }

        /// <summary>
        /// Gets/sets if primary image should be used for the
        /// content type. The default value is true.
        /// </summary>
        public bool UsePrimaryImage { get; set; } = true;

        /// <summary>
        /// Gets/sets if excerpt should be used for the
        /// content type. The default value is true.
        /// </summary>
        public bool UseExcerpt { get; set; } = true;

        /// <summary>
        /// Gets/sets if excerpt should in HTML-format. The
        /// default value is false.
        /// </summary>
        public bool UseHtmlExcerpt { get; set; }

        /// <summary>
        /// Gets/sets if this is publicly available content
        /// that can be published.
        /// </summary>
        public bool UsePublish { get; set; }

        /// <summary>
        /// Gets/sets if tags should be used for the content type.
        /// </summary>
        public bool UseTags { get; set; }

        /// <summary>
        /// Gets/sets if the content should be translatable.
        /// </summary>
        public bool UseTranslations { get; set; }
    }
}