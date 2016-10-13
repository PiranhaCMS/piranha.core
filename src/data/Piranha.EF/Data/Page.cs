/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Collections.Generic;

namespace Piranha.EF.Data
{
    public sealed class Page : Models.PageBase, IModel, ISlug, ICreated, IModified
    {
        #region Properties
        /// <summary>
        /// Gets/sets the available fields.
        /// </summary>
        public IList<PageField> Fields { get; set; }
        #endregion

        #region Navigation properties
        /// <summary>
        /// Gets/sets the page type.
        /// </summary>
        public PageType PageType { get; set; }
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Page() {
            Fields = new List<PageField>();
        }
    }
}
