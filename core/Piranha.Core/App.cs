/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using AutoMapper;
using Piranha.Extend;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Piranha
{
	/// <summary>
	/// The main Piranha application object.
	/// </summary>
	public sealed class App {
		#region Members
		/// <summary>
		/// The private singleton instance.
		/// </summary>
		private static readonly App instance = new App();

		/// <summary>
		/// The private application state.
		/// </summary>
		private bool isInitialized = false;

		/// <summary>
		/// The private initialization mutex.
		/// </summary>
		private object mutex = new object();

		/// <summary>
		/// The private extension manager
		/// </summary>
		private Extend.ExtensionManager extensionManager;

		/// <summary>
		/// The private auto mapper configuration.
		/// </summary>
		private MapperConfiguration mapper;
		#endregion

		#region Static properties
		/// <summary>
		/// Gets the current extension manager.
		/// </summary>
		public static ExtensionManager ExtensionManager {
			get { return instance.extensionManager; }
		}

		/// <summary>
		/// Gets a mapper instance.
		/// </summary>
		public static IMapper Mapper {
			get { return instance.mapper.CreateMapper(); }
		}
		#endregion

		/// <summary>
		/// Default private constructor.
		/// </summary>
		private App() { }

		/// <summary>
		/// Initializes the application.
		/// </summary>
		/// <param name="configurate">Action for configurating the application</param>
		public static void Init(Action<AppConfig> configurate = null) {
			var config = new AppConfig();

			if (configurate != null)
				configurate(config);
			instance.Initialize(config);
		}

		#region Private methods
		/// <summary>
		/// Initializes the application instance.
		/// </summary>
		private void Initialize(AppConfig config) {
			if (!isInitialized) {
				lock (mutex) {
					if (!isInitialized) {
						// Create & compose the extension manager
						extensionManager = new ExtensionManager().Compose();

						// Setup auto mapper
						mapper = new MapperConfiguration(cfg => {
							cfg.CreateMap<Data.Page, Models.PageModel>()
								.ForMember(m => m.Permalink, o => o.Ignore())
								.ForMember(m => m.IsStartPage, o => o.Ignore())
								.ForMember(m => m.Regions, o => o.Ignore());
							cfg.CreateMap<Data.Page, Models.SiteMapModel>()
								.ForMember(m => m.Permalink, o => o.Ignore())
								.ForMember(m => m.Level, o => o.Ignore())
								.ForMember(m => m.Children, o => o.Ignore());
							cfg.CreateMap<Data.Post, Models.PostListModel>()
								.ForMember(m => m.Permalink, o => o.Ignore());
							cfg.CreateMap<Data.Post, Models.PostModel>()
								.ForMember(m => m.Permalink, o => o.Ignore())
								.ForMember(m => m.Regions, o => o.Ignore());
						});
						mapper.AssertConfigurationIsValid();

						isInitialized = true;
					}
				}
			}
		}
		#endregion
	}
}
