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

namespace Piranha.Extend
{
	/// <summary>
	/// Attribute for marking a class as an extension.
	/// </summary>
    public sealed class ExtensionAttribute : Attribute
    {
		/// <summary>
		/// Gets/sets the current extension types.
		/// </summary>
		public ExtensionType Types { get; set; }
    }
}
