/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using Newtonsoft.Json;
using Piranha.Models;

namespace Piranha.Extend.Fields
{
    [Field(Name = "Post", Shorthand = "Post")]
    public class PostField : IField
    {
        /// <summary>
        /// Gets/sets the media id.
        /// </summary>
        /// <returns></returns>
        public Guid? Id { get; set; }

        /// <summary>
        /// Gets/sets the related post object.
        /// </summary>
        [JsonIgnore]
        public DynamicPost Post { get; private set; }

        /// <summary>
        /// Gets if the field has a post object available.
        /// </summary>
        public bool HasValue => Post != null;

        /// <summary>
        /// Gets the list item title if this field is used in
        /// a collection regions.
        /// </summary>
        public virtual string GetTitle() {
            return Post?.Title;
        }

        /// <summary>
        /// Initializes the field for client use.
        /// </summary>
        /// <param name="api">The current api</param>
        public virtual void Init(IApi api) {
            if (!Id.HasValue)
            {
                return;
            }

            Post = api.Posts.GetById(Id.Value);

            if (Post == null) {
                // The post has been removed, remove the
                // missing id.
                Id = null;
            }
        }

        /// <summary>
        /// Initializes the field for manager use. This
        /// method can be used for loading additional meta
        /// data needed.
        /// </summary>
        /// <param name="api">The current api</param>
        public virtual void InitManager(IApi api) { 
            Init(api);
        }

        /// <summary>
        /// Gets the referenced post.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <returns>The referenced post</returns>
        public virtual T GetPost<T>(IApi api) where T : Post<T> {
            if (Id.HasValue)
                return api.Posts.GetById<T>(Id.Value);
            return null;
        }

        /// <summary>
        /// Implicit operator for converting a Guid id to a field.
        /// </summary>
        /// <param name="guid">The guid value</param>
        public static implicit operator PostField(Guid guid) {
            return new PostField { Id = guid };
        }

        /// <summary>
        /// Implicit operator for converting a post object to a field.
        /// </summary>
        /// <param name="post">The post object</param>
        public static implicit operator PostField(PostBase post) {
            return new PostField { Id = post.Id };
        }
    }
}