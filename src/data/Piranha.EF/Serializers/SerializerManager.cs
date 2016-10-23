/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Piranha.EF.Serializers
{
    /// <summary>
    /// Handles field serialization.
    /// </summary>
    public class SerializerManager
    {
        #region Members
        /// <summary>
        /// The private items dictionary.
        /// </summary>
        private readonly Dictionary<Type, ISerializer> items = new Dictionary<Type, ISerializer>();
        #endregion

        /// <summary>
        /// Registers a new serializer for the specified type.
        /// </summary>
        /// <typeparam name="T">The field type</typeparam>
        /// <param name="serializer">The serializer</param>
        public void Register<T>(ISerializer serializer) {
            items[typeof(T)] = serializer;
        }

        /// <summary>
        /// Unregisters the serializer for the specified type.
        /// </summary>
        /// <typeparam name="T">The field type</typeparam>
        public void UnRegister<T>() {
            if (items.ContainsKey(typeof(T)))
                items.Remove(typeof(T));
        }

        /// <summary>
        /// Gets the serializer for the specified type.
        /// </summary>
        /// <typeparam name="T">The field type</typeparam>
        /// <returns>The serializer</returns>
        public ISerializer Get<T>() {
            return Get(typeof(T));
        }

        /// <summary>
        /// Gets the serializer for the specified type.
        /// </summary>
        /// <param name="type">The field type</param>
        /// <returns>The serializer</returns>
        public ISerializer Get(Type type) {
            if (items.ContainsKey(type))
                return items[type];
            return null;
        }

        /// <summary>
        /// Serializes the given object.
        /// </summary>
        /// <param name="obj">The object</param>
        /// <returns>The serialized object</returns>
        public string Serialize(object obj) {
            var serializer = Get(obj.GetType());

            if (serializer != null)
                return serializer.Serialize(obj);
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// Deserializes the given string.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="str">The serialized string</param>
        /// <returns>The object</returns>
        public object Deserialize(Type type, string str) {
            var serializer = Get(type);

            if (serializer != null)
                return serializer.Deserialize(str);
            return JsonConvert.DeserializeObject(str, type);
        }
    }
}
