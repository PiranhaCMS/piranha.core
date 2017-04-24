/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Newtonsoft.Json;

namespace Piranha.Extend.Fields
{
    public class MediaFieldBase : IField
    {
        /// <summary>
        /// Gets the list item title if this field is used in
        /// a collection regions.
        /// </summary>
        public virtual string GetTitle() {
            if (Media != null)
                return Media.Filename;
            return null;
        }

        /// <summary>
        /// Gets/sets the media id.
        /// </summary>
        /// <returns></returns>
        public string Id { get; set; }

        /// <summary>
        /// Gets/sets the related media object.
        /// </summary>
        [JsonIgnore]
        public Data.Media Media { get; private set; }

        /// <summary>
        /// Gets if the field has a media object available.
        /// </summary>
        public bool HasValue {
            get { return Media != null; }
        }

        /// <summary>
        /// Initializes the field for client use.
        /// </summary>
        /// <param name="api">The current api</param>
        public virtual void Init(Api api) { 
            if (!string.IsNullOrWhiteSpace(Id)) {
                Media = api.Media.GetById(Id);

                if (Media == null) {
                    // The image has been removed, remove the
                    // missing id.
                    Id = null;
                }
            }
        }

        /// <summary>
        /// Initializes the field for manager use. This
        /// method can be used for loading additional meta
        /// data needed.
        /// </summary>
        /// <param name="api">The current api</param>
        public virtual void InitManager(Api api) { 
            Init(api);
        }
    }
}
