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
	/// A registered extension.
	/// </summary>
    public sealed class ExtensionInfo
    {
		#region Properties
		/// <summary>
		/// Gets/sets the CLR type.
		/// </summary>
		public Type CLRType { get; set; }

		/// <summary>
		/// Gets/sets the extension types.
		/// </summary>
		public ExtensionType Types { get; set; }
		#endregion

		/// <summary>
		/// Default internal constructor.
		/// </summary>
		internal ExtensionInfo() { }

		/// <summary>
		/// Creates a new instance of the extension.
		/// </summary>
		/// <returns></returns>
		public IExtension CreateInstance() {
			return (IExtension)Activator.CreateInstance(CLRType);
		}
    }
}
