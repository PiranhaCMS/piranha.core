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
using System.Collections.Generic;

namespace Piranha.Extend
{
    public sealed class SerializerManager
    {
        /// <summary>
        /// The currently available serializers.
        /// </summary>
        private readonly Dictionary<Type, ISerializer> _serializers;

        /// <summary>
        /// Gets the serializer for the specified type.
        /// </summary>
        public ISerializer this[Type type] => _serializers.TryGetValue(type, out var serializer) ? serializer : null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SerializerManager()
        {
            _serializers = new Dictionary<Type, ISerializer>();
        }

        /// <summary>
        /// Registers the given serializer for the specified type
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Register<T>(ISerializer serializer)
        {
            _serializers[typeof(T)] = serializer;
        }

        /// <summary>
        /// Unregisters the current serializer for the specified type
        /// </summary>
        public void UnRegister<T>()
        {
            _serializers.Remove(typeof(T));
        }
    }
}