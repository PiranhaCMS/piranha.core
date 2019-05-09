/*
 * Copyright (c) 2018-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using Piranha.Models;

namespace Piranha.Extend.Fields
{
    [FieldType(Name = "Audio", Shorthand = "Audio")]
    public class AudioField : MediaFieldBase<AudioField>
    {
        /// <summary>
        /// Implicit operator for converting a Guid id to a field.
        /// </summary>
        /// <param name="str">The string value</param>
        public static implicit operator AudioField(Guid guid)
        {
            return new AudioField { Id = guid };
        }

        /// <summary>
        /// Implicit operator for converting a media object to a field.
        /// </summary>
        /// <param name="media">The media object</param>
        public static implicit operator AudioField(Media media)
        {
            return new AudioField { Id = media.Id };
        }

        /// <summary>
        /// Impicit operator for converting the field to an url string.
        /// </summary>
        /// <param name="image">The Audio field</param>
        public static implicit operator string(AudioField audio)
        {
            if (audio.Media != null)
            {
                return audio.Media.PublicUrl;
            }
            return "";
        }
    }
}