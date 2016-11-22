/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace Piranha.Manager.Binders
{
    /// <summary>
    /// A type used when trying to bind an abstract type
    /// withing the Piranha manager.
    /// </summary>
    internal sealed class AbstractBinderType
    {
        /// <summary>
        /// The type.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// The model binder.
        /// </summary>
        public IModelBinder Binder { get; set; }
    }
}
