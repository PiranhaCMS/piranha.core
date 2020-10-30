/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using Piranha.Models;

namespace Piranha.AttributeBuilder
{
    /// <summary>
    /// Class for building and importing site types.
    /// </summary>
    [NoCoverage]
    [Obsolete("SiteTypeBuilder is obsolete and has been replaced with ContentTypeBuilder", true)]
    public class SiteTypeBuilder : ContentTypeBuilder<SiteTypeBuilder, SiteType>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public SiteTypeBuilder(IApi api)
        {
        }
    }
}
