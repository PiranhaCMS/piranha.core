/*
 * Copyright (c) 2016-2018 Håkan Edling
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
    /// Generic post model.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    public class Post<T> : PostBase where T : Post<T>
    {
        /// <summary>
        /// Creates a new post model using the given post type id.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="typeId">The unique post type id</param>
        /// <returns>The new model</returns>
        public static T Create(IApi api, string typeId = null)
        {
            return api.Posts.Create<T>(typeId);
        }
    }
}
