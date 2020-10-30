/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Models;
using System;

namespace Piranha.AttributeBuilder
{
    /// <summary>
    /// Abstract base class for importing a content type from attributes.
    /// </summary>
    /// <typeparam name="T">The builder type</typeparam>
    /// <typeparam name="TType">The content type</typeparam>
    [NoCoverage]
    public abstract class ContentTypeBuilder<T, TType>
        where T : ContentTypeBuilder<T, TType>
        where TType : ContentTypeBase
    {
        /// <summary>
        /// Adds a new type to build page types from
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The builder</returns>
        public T AddType(Type type)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Builds the page types.
        /// </summary>
        public virtual T Build()
        {
            throw new NotImplementedException();
        }
    }
}
