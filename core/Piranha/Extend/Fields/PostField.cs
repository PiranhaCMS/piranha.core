/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Newtonsoft.Json;
using System;

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
        public Models.DynamicPost Post { get; private set; }

        /// <summary>
        /// Gets if the field has a media object available.
        /// </summary>
        public bool HasValue {
            get { return Post != null; }
        }

        /// <summary>
        /// Gets the list item title if this field is used in
        /// a collection regions.
        /// </summary>
        public virtual string GetTitle() {
            if (Post != null)
                return Post.Title;
            return null;
        }

        /// <summary>
        /// Initializes the field for client use.
        /// </summary>
        /// <param name="api">The current api</param>
        public virtual void Init(IApi api) { 
            if (Id.HasValue) {
                Post = api.Posts.GetById(Id.Value);

                if (Post == null) {
                    // The image has been removed, remove the
                    // missing id.
                    Id = null;
                }
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
        /// Implicit operator for converting a Guid id to a field.
        /// </summary>
        /// <param name="str">The string value</param>
        public static implicit operator PostField(Guid guid) {
            return new PostField() { Id = guid };
        }

        /// <summary>
        /// Implicit operator for converting a post object to a field.
        /// </summary>
        /// <param name="media">The media object</param>
        public static implicit operator PostField(Models.PostBase post) {
            return new PostField() { Id = post.Id };
        }
    }
}