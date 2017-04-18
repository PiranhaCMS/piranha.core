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
    [Field(Name = "Image", Shorthand = "Image")]
    public class ImageField : SimpleField<string>
    {
        /// <summary>
        /// Gets/sets the related image object.
        /// </summary>
        [JsonIgnore]
        public Data.Media Image { get; private set; }

        /// <summary>
        /// Gets/sets the filename of the image object.
        /// </summary>
        /// <returns></returns>
        [JsonIgnore]
        public string Filename { get; set; }

        /// <summary>
        /// Implicit operator for converting a string id to a field.
        /// </summary>
        /// <param name="str">The string value</param>
        public static implicit operator ImageField(string str) {
            return new ImageField() { Value = str };
        }

        /// <summary>
        /// Implicit operator for converting a media object to a field.
        /// </summary>
        /// <param name="media">The media object</param>
        public static implicit operator ImageField(Data.Media media) {
            return new ImageField() { Value = media.Id };
        }

        /// <summary>
        /// Initializes the field for client use.
        /// </summary>
        /// <param name="api">The current api</param>
        public override void Init(Api api) { 
            if (!string.IsNullOrWhiteSpace(Value)) {
                Image = api.Media.GetById(Value);

                if (Image != null) {
                    Filename = Image.Filename;
                } else {
                    // The image has been removed, remove the
                    // missing id.
                    Value = null;
                }
            }
        }

        /// <summary>
        /// Initializes the field for manager use. This
        /// method can be used for loading additional meta
        /// data needed.
        /// </summary>
        /// <param name="api">The current api</param>
        public override void InitManager(Api api) { 
            Init(api);
        }
    }
}