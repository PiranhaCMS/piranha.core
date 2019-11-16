/*
 * Copyright (c) 2017-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace Piranha.Data
{
    [Serializable]
    public sealed class Media : Models.MediaBase
    {
        /// <summary>
        /// Gets/sets the optional folder.
        /// </summary>
        public MediaFolder Folder { get; set; }

        /// <summary>
        /// Gets/sets the available versions.
        /// </summary>
        public IList<MediaVersion> Versions { get; set; } = new List<MediaVersion>();

        public static implicit operator Models.Media(Data.Media m)
        {
            return m == null? null : new Models.Media
            {
                Id = m.Id,
                FolderId = m.FolderId,
                Type = m.Type,
                Filename = m.Filename,
                ContentType = m.ContentType,
                Size = m.Size,
                Width = m.Width,
                Height = m.Height,
                Created = m.Created,
                LastModified = m.LastModified,
                Versions = m.Versions.Select(v => new Models.MediaVersion
                {
                    Id = v.Id,
                    Size = v.Size,
                    Width = v.Width,
                    Height = v.Height,
                    FileExtension = v.FileExtension
                }).ToList()
            };
        }

        public static implicit operator Media(Models.Media m)
        {
            return new Media
            {
                Id = m.Id,
                FolderId = m.FolderId,
                Type = m.Type,
                Filename = m.Filename,
                ContentType = m.ContentType,
                Size = m.Size,
                Width = m.Width,
                Height = m.Height,
                Created = m.Created,
                LastModified = m.LastModified,
                Versions = m.Versions.Select(v => new MediaVersion
                {
                    Id = v.Id,
                    Size = v.Size,
                    Width = v.Width,
                    Height = v.Height,
                    FileExtension = v.FileExtension
                }).ToList()
            };
        }
    }
}