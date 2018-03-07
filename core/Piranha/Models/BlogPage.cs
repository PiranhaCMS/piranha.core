/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

namespace Piranha.Models
{
    /// <summary>
    /// Base class for blog pages.
    /// </summary>
    public class BlogPage<T> : GenericPage<T>, IBlogPage where T : BlogPage<T>
    {
        /// <summary>
        /// Gets/sets the post archive.
        /// </summary>
        public PostArchive Archive { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BlogPage()
        {
            Archive = new PostArchive();
        }
    }

    /// <summary>
    /// Interface for registering the basic blog page 
    /// content type.
    /// </summary>
    public interface IBlogPage { }
}