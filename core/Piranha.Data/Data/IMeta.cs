/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.dnx
 * 
 */

namespace Piranha.Data
{
	/// <summary>
	/// Interface for models exposing meta data.
	/// </summary>
    public interface IMeta
    {
		/// <summary>
		/// Gets/sets the optional meta title.
		/// </summary>
		string MetaTitle { get; set; }

		/// <summary>
		/// Gets/sets the optional meta keywords.
		/// </summary>
		string MetaKeywords { get; set; }

		/// <summary>
		/// Gets/sets the optional meta description.
		/// </summary>
		string MetaDescription { get; set; }
    }
}
