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
    public class MediaFolder : Piranha.Models.MediaFolder, IModel
    {
        #region Navigation properties
        /// <summary>
        /// Gets/sets the media in this folder.
        /// </summary>
        public IList<Media> Media { get; set;}
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MediaFolder() {
            Media = new List<Media>();
        }
    }
}