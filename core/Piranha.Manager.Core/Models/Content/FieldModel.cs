/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using Piranha.Extend;

namespace Piranha.Manager.Models.Content
{
    /// <summary>
    /// Edit model for a field.
    /// </summary>
    public class FieldModel
    {
        /// <summary>
        /// Gets/sets the field model.
        /// </summary>
        public IField Model { get; set; }

        /// <summary>
        /// Gets/sets the meta information.
        /// </summary>
        public FieldMeta Meta { get; set; }
    }
}