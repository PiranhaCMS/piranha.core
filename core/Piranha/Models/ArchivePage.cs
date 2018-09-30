/*
 * Copyright (c) 2017-2018 Håkan Edling
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
    [Obsolete("BlogPage has been renamed to ArchivePage, please update your code.", false)]
    public class BlogPage<T> : ArchivePage<T> where T : BlogPage<T> { }

    /// <summary>
    /// Base class for archive pages.
    /// </summary>
    public class ArchivePage<T> : GenericPage<T>, IArchivePage where T : ArchivePage<T>
    {
        /// <summary>
        /// Gets/sets the post archive.
        /// </summary>
        public PostArchive Archive { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ArchivePage()
        {
            Archive = new PostArchive();
        }
    }


    [Obsolete]
    public interface IBlogPage : IArchivePage { }
    
    /// <summary>
    /// Interface for registering the basic archive page 
    /// content type.
    /// </summary>
    public interface IArchivePage { }
}