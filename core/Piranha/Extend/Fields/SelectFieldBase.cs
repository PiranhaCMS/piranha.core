/*
 * Copyright (c) .NET Foundation and Contributors
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

namespace Piranha.Extend.Fields
{
    /// <summary>
    /// Base class for all SelectFields.
    /// </summary>
    public abstract class SelectFieldBase : IField
    {
        /// <summary>
        /// Gets the type of the enum.
        /// </summary>
        [JsonIgnore]
        public abstract Type EnumType { get; }

        /// <summary>
        /// Gets/sets the value of the current enum value.
        /// </summary>
        [JsonIgnore]
        public abstract string EnumValue { get; set; }

        /// <summary>
        /// Gets the available items to choose from. Primarily used
        /// from the manager interface.
        /// </summary>
        [JsonIgnore]
        public abstract List<SelectFieldItem> Items { get; }

        /// <summary>
        /// Gets the list item title if this field is used in
        /// a collection regions.
        /// </summary>
        public abstract string GetTitle();

        /// <summary>
        /// Initializes the field for client use.
        /// </summary>
        /// <param name="api">The current api</param>
        public abstract void Init(IApi api);
    }
}