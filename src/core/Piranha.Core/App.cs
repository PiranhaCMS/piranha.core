/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Collections.Generic;

namespace Piranha
{
	/// <summary>
	/// The main application object.
	/// </summary>
    public sealed class App
    {
		#region Members
		private static readonly App instance = new App();
		private readonly object mutex = new object();
		private bool isInitialized = false;
		private readonly IList<Extend.IModule> modules;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the currently registred modules.
		/// </summary>
		public static IList<Extend.IModule> Modules {
			get { return instance.modules; }
		}
		#endregion

		/// <summary>
		/// Default private constructor.
		/// </summary>
		private App() {
			modules = new List<Extend.IModule>();
		}

		/// <summary>
		/// Initializes the application object.
		/// </summary>
		/// <param name="modules">The modules to use</param>
		public static void Init(params Extend.IModule[] modules) {
			instance.Initialize(modules);
		}

		/// <summary>
		/// Initializes the application object.
		/// </summary>
		/// <param name="modules">The modules to use</param>
		private void Initialize(Extend.IModule[] modules = null) {
			if (!isInitialized) {
				lock (mutex) {
					if (!isInitialized) {
						// Compose & initialize modules
						foreach (var module in modules) {
							module.Init();
							this.modules.Add(module);
						}

						isInitialized = true;
					}
				}
			}
		}
    }
}
