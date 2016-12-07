/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

namespace Piranha.EF.Data 
{
    public class Media : Piranha.Models.Media, IModel, ICreated, IModified
    {
        #region Navigation properties
        /// <summary>
        /// Gets/sets the optional media folder.
        /// </summary>
        public MediaFolder Folder { get; set; }
        #endregion
    }
}