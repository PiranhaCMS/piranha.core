/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Extend
{
    /// <summary>
    /// Interface for creating a field serializer.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Serializes the given object.
        /// </summary>
        /// <param name="obj">The object</param>
        /// <returns>The serialized value</returns>
        string Serialize(object obj);

        /// <summary>
        /// Deserializes the given string.
        /// </summary>
        /// <param name="str">The serialized value</param>
        /// <returns>The object</returns>
        object Deserialize(string str);
    }
}
