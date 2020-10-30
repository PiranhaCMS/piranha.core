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

namespace Piranha.AttributeBuilder
{
    /// <summary>
    /// Attribute for adding a custom editor to a post type.
    /// </summary>
    [NoCoverage]
    [Obsolete("PostTypeEditorAttribute is obsolete and has been replaced with ContentTypeEditorAttribute", true)]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PostTypeEditorAttribute : ContentTypeEditorAttribute { }
}
