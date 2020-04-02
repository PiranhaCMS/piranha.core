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
using System.Threading.Tasks;

namespace Piranha.Extend.Fields
{
    [FieldType(Name = "Page", Shorthand = "Page", Component = "page-field")]
    public class PageField : IField, IEquatable<PageField>
    {
        /// <summary>
        /// Gets/sets the media id.
        /// </summary>
        /// <returns></returns>
        public Guid? Id { get; set; }

        /// <summary>
        /// Gets/sets the related page object.
        /// </summary>
        public Models.PageInfo Page { get; private set; }

        /// <summary>
        /// Gets if the field has a page object available.
        /// </summary>
        public bool HasValue => Page != null;

        /// <summary>
        /// Gets the list item title if this field is used in
        /// a collection regions.
        /// </summary>
        public virtual string GetTitle()
        {
            return Page?.Title;
        }

        /// <summary>
        /// Initializes the field for client use.
        /// </summary>
        /// <param name="api">The current api</param>
        public virtual async Task Init(IApi api)
        {
            if (Id.HasValue)
            {
                Page = await api.Pages
                    .GetByIdAsync<Models.PageInfo>(Id.Value)
                    .ConfigureAwait(false);

                if (Page == null)
                {
                    // The page has been removed, remove the
                    // missing id.
                    Id = null;
                }
            }
        }

        /// <summary>
        /// Gets the referenced page.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <returns>The referenced page</returns>
        public virtual Task<T> GetPageAsync<T>(IApi api) where T : Models.GenericPage<T>
        {
            if (Id.HasValue)
            {
                return api.Pages.GetByIdAsync<T>(Id.Value);
            }
            return null;
        }

        /// <summary>
        /// Implicit operator for converting a Guid id to a field.
        /// </summary>
        /// <param name="guid">The guid value</param>
        public static implicit operator PageField(Guid guid)
        {
            return new PageField { Id = guid };
        }

        /// <summary>
        /// Implicit operator for converting a page object to a field.
        /// </summary>
        /// <param name="page">The page object</param>
        public static implicit operator PageField(Models.PageBase page)
        {
            return new PageField { Id = page.Id };
        }

        /// <summary>
        /// Gets the hash code for the field.
        /// </summary>
        public override int GetHashCode()
        {
            return Id.HasValue ? Id.GetHashCode() : 0;
        }

        /// <summary>
        /// Checks if the given object is equal to the field.
        /// </summary>
        /// <param name="obj">The object</param>
        /// <returns>True if the fields are equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is PageField field)
            {
                return Equals(field);
            }
            return false;
        }

        /// <summary>
        /// Checks if the given field is equal to the field.
        /// </summary>
        /// <param name="obj">The field</param>
        /// <returns>True if the fields are equal</returns>
        public virtual bool Equals(PageField obj)
        {
            if (obj == null)
            {
                return false;
            }
            return Id == obj.Id;
        }

        /// <summary>
        /// Checks if the fields are equal.
        /// </summary>
        /// <param name="field1">The first field</param>
        /// <param name="field2">The second field</param>
        /// <returns>True if the fields are equal</returns>
        public static bool operator ==(PageField field1, PageField field2)
        {
            if ((object) field1 != null && (object) field2 != null)
            {
                return field1.Equals(field2);
            }

            if ((object)field1 == null && (object)field2 == null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the fields are not equal.
        /// </summary>
        /// <param name="field1">The first field</param>
        /// <param name="field2">The second field</param>
        /// <returns>True if the fields are equal</returns>
        public static bool operator !=(PageField field1, PageField field2)
        {
            return !(field1 == field2);
        }
    }
}