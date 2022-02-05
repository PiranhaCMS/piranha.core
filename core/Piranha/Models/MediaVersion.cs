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

namespace Piranha.Models
{
    [Serializable]
    public class MediaVersion
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the file size in bytes.
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Gets/sets the width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets/sets the optional height.
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// Gets/sets the file extension of the generated version.
        /// </summary>
        public string FileExtension { get; set; }
    }
}