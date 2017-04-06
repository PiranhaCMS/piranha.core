/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System;

namespace Piranha.Data
{
    /// <summary>
    /// String parameter.
    /// </summary>
    public sealed class Param : Param<string> { }

    /// <summary>
    /// Generic system parameter.
    /// </summary>
    /// <typeparam name="T">The value type</typeparam>
    public class Param<T> : IModel, ICreated, IModified
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets/sets the unique key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets/sets the value.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Gets/sets the optional description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets/sets the created date.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets/sets the last modification date.
        /// </summary>
        public DateTime LastModified { get; set; }
    }
}
