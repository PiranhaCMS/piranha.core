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
    [Field(Name = "Page", Shorthand = "Page")]
    public class PageField : IField
    {
        /// <summary>
        /// Gets/sets the media id.
        /// </summary>
        /// <returns></returns>
        public Guid? Id { get; set; }

        /// <summary>
        /// Gets/sets the related page object.
        /// </summary>
        [JsonIgnore]
        public Models.DynamicPage Page { get; private set; }

        /// <summary>
        /// Gets if the field has a page object available.
        /// </summary>
        public bool HasValue {
            get { return Page != null; }
        }

        /// <summary>
        /// Gets the list item title if this field is used in
        /// a collection regions.
        /// </summary>
        public virtual string GetTitle() {
            if (Page != null)
                return Page.Title;
            return null;
        }

        /// <summary>
        /// Initializes the field for client use.
        /// </summary>
        /// <param name="api">The current api</param>
        public virtual void Init(IApi api) { 
            if (Id.HasValue) {
                Page = api.Pages.GetById(Id.Value);

                if (Page == null) {
                    // The page has been removed, remove the
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
        /// Gets the referenced page.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <returns>The referenced page</returns>
        public virtual T GetPage<T>(IApi api) where T : Models.GenericPage<T> {
            if (Id.HasValue)
                return api.Pages.GetById<T>(Id.Value);
            return null;
        }        

        /// <summary>
        /// Implicit operator for converting a Guid id to a field.
        /// </summary>
        /// <param name="str">The string value</param>
        public static implicit operator PageField(Guid guid) {
            return new PageField() { Id = guid };
        }

        /// <summary>
        /// Implicit operator for converting a page object to a field.
        /// </summary>
        /// <param name="page">The page object</param>
        public static implicit operator PageField(Models.PageBase page) {
            return new PageField() { Id = page.Id };
        }
    }
}