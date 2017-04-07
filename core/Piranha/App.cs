/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using HeyRed.MarkdownSharp;
using AutoMapper;
using Piranha.Extend;
using System.Reflection;

namespace Piranha
{
    /// <summary>
    /// The main application object.
    /// </summary>
    public sealed class App
    {
        #region Members
        /// <summary>
        /// The singleton app instance.
        /// </summary>
        private static readonly App instance = new App();

        /// <summary>
        /// Mutex for thread safe initialization.
        /// </summary>
        private readonly object mutex = new object();

        /// <summary>
        /// The current state of the app.
        /// </summary>
        private bool isInitialized = false;

        /// <summary>
        /// The currently registered fields.
        /// </summary>
        private AppFieldList fields;

        /// <summary>
        /// The currently registered modules.
        /// </summary>
        private AppModuleList modules;

        /// <summary>
        /// The application object mapper.
        /// </summary>
        private IMapper mapper;

        /// <summary>
        /// The application markdown converter.
        /// </summary>
        private Markdown markdown;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the currently registered field types.
        /// </summary>
        public static AppFieldList Fields {
            get { return instance.fields; }
        }

        /// <summary>
        /// Gets the currently registred modules.
        /// </summary>
        public static AppModuleList Modules {
            get { return instance.modules; }
        }

        /// <summary>
        /// Gets the application object mapper.
        /// </summary>
        public static IMapper Mapper {
            get { return instance.mapper; }
        }

        /// <summary>
        /// Gets the markdown converter.
        /// </summary>
        public static Markdown Markdown {
            get { return instance.markdown; }
        }

        /// <summary>
        /// Gets the binding flags for retrieving a region from a
        /// strongly typed model.
        /// </summary>
        public static BindingFlags PropertyBindings {
            get { return BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance; }
        }
        #endregion

        /// <summary>
        /// Default private constructor.
        /// </summary>
        private App() {
            fields = new AppFieldList();
            modules = new AppModuleList();
        }

        /// <summary>
        /// Initializes the application object.
        /// </summary>
        /// <param name="api">The current api</param>
        public static void Init(Api api) {
            instance.Initialize(api);
        }

        #region Private methods
        /// <summary>
        /// Initializes the application object.
        /// </summary>
        /// <param name="api">The current api</param>
        private void Initialize(Api api) {
            if (!isInitialized) {
                lock (mutex) {
                    if (!isInitialized) {
                        // Configure object mapper
                        var mapperConfig = new MapperConfiguration(cfg => {
                            cfg.CreateMap<Data.Page, Models.PageBase>()
                                .ForMember(p => p.TypeId, o => o.MapFrom(m => m.PageTypeId));
                            cfg.CreateMap<Models.PageBase, Data.Page>()
                                .ForMember(p => p.PageTypeId, o => o.MapFrom(m => m.TypeId))
                                .ForMember(p => p.Fields, o => o.Ignore())
                                .ForMember(p => p.Created, o => o.Ignore())
                                .ForMember(p => p.LastModified, o => o.Ignore());
                            cfg.CreateMap<Data.Page, Models.SitemapItem>()
                                .ForMember(p => p.MenuTitle, o => o.Ignore())
                                .ForMember(p => p.Level, o => o.Ignore())
                                .ForMember(p => p.Items, o => o.Ignore())
                                .ForMember(p => p.Permalink, o => o.MapFrom(d => string.IsNullOrWhiteSpace(d.ParentId) && d.SortOrder == 0 ? "/" : d.Slug));
                        });
                        mapperConfig.AssertConfigurationIsValid();
                        mapper = mapperConfig.CreateMapper();

                        // Compose field types
                        fields.Register<Extend.Fields.HtmlField>();
                        fields.Register<Extend.Fields.MarkdownField>();
                        fields.Register<Extend.Fields.StringField>();
                        fields.Register<Extend.Fields.TextField>();

                        // Create markdown converter
                        markdown = new Markdown();

                        // Initialize all modules
                        foreach (var module in this.modules) {
                            module.Instance.Init();
                        }

                        isInitialized = true;
                    }
                }
            }
        }
        #endregion
    }
}
