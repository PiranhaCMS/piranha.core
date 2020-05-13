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
using Microsoft.AspNetCore.Http;

namespace Piranha.Manager.Models
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
        public IEnumerable<IFormFile> Uploads { get; set; } = new List<IFormFile>();
    }
}
