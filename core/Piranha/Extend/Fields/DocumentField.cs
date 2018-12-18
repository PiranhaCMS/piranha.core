/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;

namespace Piranha.Extend.Fields
{
    [FieldType(Name = "Document", Shorthand = "Document")]
    public class DocumentField : MediaFieldBase<DocumentField>
    {
        /// <summary>
        /// Implicit operator for converting a Guid id to a field.
        /// </summary>
        /// <param name="str">The string value</param>
        public static implicit operator DocumentField(Guid guid)
        {
            return new DocumentField { Id = guid };
        }

        /// <summary>
        /// Implicit operator for converting a media object to a field.
        /// </summary>
        /// <param name="media">The media object</param>
        public static implicit operator DocumentField(Data.Media media)
        {
            return new DocumentField { Id = media.Id };
        }

        /// <summary>
        /// Impicit operator for converting the field to an url string.
        /// </summary>
        /// <param name="image">The document field</param>
        public static implicit operator string(DocumentField image)
        {
            if (image.Media != null)
            {
                return image.Media.PublicUrl;
            }
            return "";
        }
    }
}