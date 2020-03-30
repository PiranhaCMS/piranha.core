/*
 * Copyright (c) .NET Foundation and Contributors
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
using Newtonsoft.Json;

namespace Piranha.Data
{
    [Serializable]
    public sealed class Media : Models.MediaBase
    {
        /// <summary>
        /// Gets/sets the user defined properties serialized as JSON.
        /// </summary>
        public string Properties { get; set; }

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
                Title = m.Title,
                AltText = m.AltText,
                Description = m.Description,
                Properties = DeSerializeProperties(m.Properties),
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
                Title = m.Title,
                AltText = m.AltText,
                Description = m.Description,
                Properties = SerializeProperties(m.Properties),
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

        internal static IDictionary<string, string> DeSerializeProperties(string str)
        {
            var properties = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(str))
            {
                foreach (var prop in JsonConvert.DeserializeObject<IEnumerable<Tuple<string, string>>>(str))
                {
                    if (!string.IsNullOrEmpty(prop.Item1))
                    {
                        properties[prop.Item1] = prop.Item2;
                    }
                }
            }
            return properties;
        }

        internal static string SerializeProperties(IDictionary<string, string> properties)
        {
            var items = new List<Tuple<string, string>>();

            foreach (var key in properties.Keys)
            {
                items.Add(new Tuple<string, string>(key, properties[key]));
            }
            return JsonConvert.SerializeObject(items);
        }
    }
}