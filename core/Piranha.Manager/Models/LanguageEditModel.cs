/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Collections.Generic;
using Piranha.Models;

namespace Piranha.Manager.Models
{
    /// <summary>
    /// Language modal edit model.
    /// </summary>
    public class LanguageEditModel
    {
        /// <summary>
        /// Gets/sets the available languages
        /// </summary>
        public IEnumerable<Language> Items { get; set; } = new List<Language>();
    }
}