/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Piranha.Extend;

namespace Piranha.Hooks
{
	/// <summary>
	/// The hooks available for extensions.
	/// </summary>
    public static class Extend
    {
		/// <summary>
		/// The delegates used.
		/// </summary>
		public static class Delegates
		{
			/// <summary>
			/// Delegate for adding types to the extension manager
			/// composition.
			/// </summary>
			/// <param name="mgr">The extension manager</param>
			public delegate void OnComposeDelegate(ExtensionManager mgr);
		}

		/// <summary>
		/// Called when the extension manager has composed the default types.
		/// </summary>
		public static Delegates.OnComposeDelegate OnCompose;
    }
}
