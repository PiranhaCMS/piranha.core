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
using Piranha.Extend.Fields;
using Piranha.Models;

namespace Piranha.AspNetCore.Services
{
    public interface IMediaHelper
    {
            /// <summary>
            /// Resizes the given image to the given dimensions.
            /// </summary>
            /// <param name="image"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <returns></returns>
            string ResizeImage(ImageField image, int width, int? height = null);

            /// <summary>
            /// Resizes the given image to the given dimensions.
            /// </summary>
            /// <param name="image"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <returns></returns>
            string ResizeImage(Media image, int width, int? height = null);
    }
}