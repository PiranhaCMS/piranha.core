/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using Newtonsoft.Json;
using Piranha.Data;

namespace Piranha.Extend.Fields
{
    public class MediaFieldBase : IField
    {
        /// <summary>
        /// Gets the list item title if this field is used in
        /// a collection regions.
        /// </summary>
        public virtual string GetTitle()
        {
            return Media?.Filename;
        }

        /// <summary>
        /// Gets/sets the media id.
        /// </summary>
        /// <returns></returns>
        public Guid? Id { get; set; }

        /// <summary>
        /// Gets/sets the related media object.
        /// </summary>
        [JsonIgnore]
        public Media Media { get; private set; }

        /// <summary>
        /// Gets if the field has a media object available.
        /// </summary>
        public bool HasValue => Media != null;

        /// <summary>
        /// Initializes the field for client use.
        /// </summary>
        /// <param name="api">The current api</param>
        public virtual void Init(IApi api)
        {
            if (!Id.HasValue)
            {
                return;
            }

            Media = api.Media.GetById(Id.Value);

            if (Media == null)
            {
                // The image has been removed, remove the
                // missing id.
                Id = null;
            }
        }

        /// <summary>
        /// Initializes the field for manager use. This
        /// method can be used for loading additional meta
        /// data needed.
        /// </summary>
        /// <param name="api">The current api</param>
        public virtual void InitManager(IApi api)
        {
            Init(api);
        }
    }
}
