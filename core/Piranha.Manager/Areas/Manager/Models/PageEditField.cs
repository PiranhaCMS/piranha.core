/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Piranha.Extend;
using Piranha.Models;

namespace Piranha.Areas.Manager.Models
{
    /// <summary>
    /// A page field.
    /// </summary>
    public class PageEditField
    {
        /// <summary>
        /// Gets/sets the id.
        /// </summary>
        /// <returns></returns>
        public string Id { get; set; }

        /// <summary>
        /// Gets/sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the CLR type of the field value.
        /// </summary>
        public string ClrType { get; set; }

        /// <summary>
        /// Gets/sets the possible options.
        /// </summary>
        public FieldOption Options { get; set; }

        /// <summary>
        /// Gets/sets the field value.
        /// </summary>
        public IField Value { get; set; }
    }
}