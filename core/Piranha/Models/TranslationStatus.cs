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

namespace Piranha.Models
{
    /// <summary>
    /// Content translation status for a specific content model.
    /// </summary>
    [Serializable]
    public class TranslationStatus
    {
        /// <summary>
        /// An translation status item.
        /// </summary>
        [Serializable]
        public class TranslationStatusItem
        {
            /// <summary>
            /// Gets/sets the languge id.
            /// </summary>
            public Guid LanguageId { get; set; }

            /// <summary>
            /// Gets/sets the language title.
            /// </summary>
            public string LanguageTitle { get; set; }

            /// <summary>
            /// Gets/sets if the language is up to date with the
            /// default master language.
            /// </summary>
            public bool IsUpToDate { get; set; }
        }

        /// <summary>
        /// Gets/sets the unique content id.
        /// </summary>
        public Guid ContentId { get; set; }

        /// <summary>
        /// Gets/sets if all of the translations is up to date.
        /// </summary>
        public bool IsUpToDate { get; set; }

        /// <summary>
        /// Gets/sets the number of up to date translations.
        /// </summary>
        public int UpToDateCount { get; set; }

        /// <summary>
        /// Gets/sets the total number of translations.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets/sets the status items for the available languages.
        /// </summary>
        public IList<TranslationStatusItem> Translations { get; set; } = new List<TranslationStatusItem>();
    }
}
