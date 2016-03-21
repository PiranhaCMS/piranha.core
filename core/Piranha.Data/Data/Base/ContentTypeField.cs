/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;

namespace Piranha.Data.Base
{
	/// <summary>
	/// Base class for creating content type fields.
	/// </summary>
	/// <typeparam name="T">The content type</typeparam>
	public abstract class ContentTypeField<T> : IModel
	{
		#region Properties
		/// <summary>
		/// Gets/sets the unique id.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Gets/sets the id of the parent content type.
		/// </summary>
		public Guid TypeId { get; set; }

		/// <summary>
		/// Gets/sets the field type.
		/// </summary>
		public FieldType FieldType { get; set; }

		/// <summary>
		/// Gets/sets the display name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets/sets the unique internal id.
		/// </summary>
		public string InternalId { get; set; }

		/// <summary>
		/// Gets/sets the CLR type of the value.
		/// </summary>
		public string CLRType { get; set; }

		/// <summary>
		/// Gets/sets the sort order.
		/// </summary>
		public int SortOrder { get; set; }
		#endregion

		#region Navigation properties
		/// <summary>
		/// Gets/sets the parent content type.
		/// </summary>
		public T Type { get; set; }
		#endregion
	}
}
