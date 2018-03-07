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
using Piranha.Extend.Fields;

namespace Piranha.Extend.Serializers
{
    public class DateFieldSerializer : ISerializer
    {
        /// <summary>
        /// Serializes the given object.
        /// </summary>
        /// <param name="obj">The object</param>
        /// <returns>The serialized value</returns>
        public string Serialize(object obj)
        {
            if (!(obj is DateField))
            {
                throw new ArgumentException("The given object doesn't match the serialization type");

            }

            var field = (DateField)obj;

            return field.Value.HasValue ? ((DateField)obj).Value?.ToString("yyyy-MM-dd HH:mm:ss") : null;
        }

        /// <summary>
        /// Deserializes the given string.
        /// </summary>
        /// <param name="str">The serialized value</param>
        /// <returns>The object</returns>
        public object Deserialize(string str)
        {
            var field = new DateField();

            if (string.IsNullOrWhiteSpace(str))
            {
                return field;
            }

            try
            {
                field.Value = DateTime.Parse(str);
            }
            catch
            {
                // ignored
            }

            return field;
        }
    }
}
