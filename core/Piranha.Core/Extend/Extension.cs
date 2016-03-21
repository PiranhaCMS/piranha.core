/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

namespace Piranha.Extend
{
	/// <summary>
	/// Base class for extensions.
	/// </summary>
    public abstract class Extension : IExtension
    {
		/// <summary>
		/// Gets the value for the client model.
		/// </summary>
		/// <returns>The value</returns>
		public virtual object GetValue() {
			return this;
		}

		/// <summary>
		/// Implicitly serializes the extension to a string.
		/// </summary>
		/// <param name="e">The extension</param>
		public static implicit operator string(Extension e) {
			return App.ExtensionManager.Serialize(e);
		}
    }
}
