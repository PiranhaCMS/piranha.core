/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        private Extend.FieldInfoList fields;
        private List<Extend.IModule> modules;
        private IList<Extend.BlockType> blockTypes;
        private IList<Extend.PageType> pageTypes;
        private Extend.IMarkdown markdown;
        private MediaTypes mediaTypes;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the currently registered field types.
        /// </summary>
        public static Extend.FieldInfoList Fields {
            get { return instance.fields; }
        }

        /// <summary>
        /// Gets the currently registred modules.
        /// </summary>
        public static IList<Extend.IModule> Modules {
            get { return instance.modules; }
        }

        /// <summary>
        /// Gets the currently registered block types.
        /// </summary>
        public static IList<Extend.BlockType> BlockTypes {
            get { return instance.blockTypes; }
        }

        /// <summary>
        /// Gets the currently registered page types.
        /// </summary>
        public static IList<Extend.PageType> PageTypes {
            get { return instance.pageTypes; }
        }

        /// <summary>
        /// Gets the binding flags for retrieving a region from a
        /// strongly typed model.
        /// </summary>
        public static BindingFlags PropertyBindings {
            get { return BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance; }
        }

        /// <summary>
        /// Gets the currently registered markdown converter.
        /// </summary>
        public static Extend.IMarkdown Markdown {
            get { return instance.markdown; }
        }

        /// <summary>
        /// Gets the currently supported media types.
        /// </summary>
        public static MediaTypes MediaTypes {
            get { return instance.mediaTypes; }
        }
        #endregion

        /// <summary>
        /// Default private constructor.
        /// </summary>
        private App() {
            fields = new Extend.FieldInfoList();
            modules = new List<Extend.IModule>();
            blockTypes = new List<Extend.BlockType>();
            pageTypes = new List<Extend.PageType>();
        }

        /// <summary>
        /// Initializes the application object.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="modules">The modules to use</param>
        public static void Init(IApi api, IConfigurationRoot config, params Extend.IModule[] modules) {
            instance.Initialize(api, config, modules);
        }

        /// <summary>
        /// Reloads the application block types.
        /// </summary>
        /// <param name="api">The current api</param>
        public static void ReloadBlockTypes(IApi api) {
            instance.blockTypes = api.BlockTypes.Get();
        }

        /// <summary>
        /// Reloads the application page types.
        /// </summary>
        /// <param name="api">The current api</param>
        public static void ReloadPageTypes(IApi api) {
            instance.pageTypes = api.PageTypes.Get();            
        }

        /// <summary>
        /// Initializes the application object.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="modules">The modules to use</param>
        private void Initialize(IApi api, IConfigurationRoot config, Extend.IModule[] modules = null) {
            if (!isInitialized) {
                lock (mutex) {
                    if (!isInitialized) {
                        // Setup media types
                        instance.mediaTypes = new MediaTypes() {
                            Documents = config.GetSection("Piranha:MediaTypes:Documents").GetChildren().Select(c => c.Value).ToList(),
                            Images = config.GetSection("Piranha:MediaTypes:Images").GetChildren().Select(c => c.Value).ToList(),
                            Videos = config.GetSection("Piranha:MediaTypes:Videos").GetChildren().Select(c => c.Value).ToList(),
                        };

                        // Register default markdown converter
                        markdown = new Extend.MarkdownSharp();

                        // Compose field types
                        fields.Register<Extend.Fields.HtmlField>();
                        fields.Register<Extend.Fields.ImageField>();
                        fields.Register<Extend.Fields.MarkdownField>();
                        fields.Register<Extend.Fields.StringField>();
                        fields.Register<Extend.Fields.TextField>();

                        // Get types
                        pageTypes = api.PageTypes.Get();
                        blockTypes = api.BlockTypes.Get();

                        // Add ad-hoc modules
                        InitializeModules(modules);

                        isInitialized = true;
                    }
                }
            }
        }

        /// <summary>
        /// Appends the array of modules to the instance and initializes them
        /// </summary>
        private void InitializeModules(Extend.IModule[] modules) {
            // Add modules if present
            if (modules != null) {
                foreach (var module in modules) {
                    this.modules.Add(module);
                }
            }

            // Initialize all modules
            foreach (var module in this.modules) {
                module.Init();
            }
        }
    }
}
