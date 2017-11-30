/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Piranha.Areas.Manager.Models
{
    /// <summary>
    /// Model for uploading a new media asset.
    /// </summary>
    public class MediaUploadModel
    {
        /// <summary>
        /// Gets/sets the optional id.
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Gets/sets the parent id.
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Gets/sets the uploaded file.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IFormFile> Uploads { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MediaUploadModel() {
            Uploads = new List<IFormFile>();
        }
    }
}