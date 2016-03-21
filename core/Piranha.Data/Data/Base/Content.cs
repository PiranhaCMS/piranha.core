/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.dnx
 * 
 */

using System;
using System.Collections.Generic;

namespace Piranha.Data.Base
{
	/// <summary>
	/// Base class for creating content.
	/// </summary>
	/// <typeparam name="T">The content type</typeparam>
	/// <typeparam name="TField">The field type</typeparam>
	/// <typeparam name="TType">The content type type</typeparam>
	/// <typeparam name="TFieldType">The field type type</typeparam>
    public abstract class Content<T, TField, TType, TFieldType> : IModel, ISlug, ICreated, IModified
		where T : Content<T, TField, TType, TFieldType>
		where TField : ContentField<T, TType, TFieldType>
		where TType : ContentType<TType, TFieldType>
		where TFieldType : ContentTypeField<TType>
    {
		#region Properties
		/// <summary>
		/// Gets/sets the unique id.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets/sets the type id.
		/// </summary>
		public Guid TypeId { get; set; }

		/// <summary>
		/// Gets/sets the optional author id.
		/// </summary>
		public Guid? AuthorId { get; set; }

		/// <summary>
		/// Gets/sets the main title.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Gets/sets the unique slug.
		/// </summary>
		public string Slug { get; set; }

		/// <summary>
		/// Gets/sets the optional route.
		/// </summary>
		public string Route { get; set; }

		/// <summary>
		/// Gets/sets the optional published date.
		/// </summary>
		public DateTime? Published { get; set; }

		/// <summary>
		/// Gets/sets the created date.
		/// </summary>
		public DateTime Created { get; set; }

		/// <summary>
		/// Gets/sets the last modification date.
		/// </summary>
		public DateTime LastModified { get; set; }
		#endregion

		#region Navigation properties
		/// <summary>
		/// Gets/sets the type.
		/// </summary>
		public TType Type { get; set; }

		/// <summary>
		/// Gets/sets the optional author.
		/// </summary>
		public Author Author { get; set; }

		/// <summary>
		/// Gets/sets the available fields.
		/// </summary>
		public IList<TField> Fields { get; set; }
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public Content() {
			Fields = new List<TField>();
		}
	}
}
