/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using AutoMapper;
using Newtonsoft.Json;
using Piranha.Extend;
using Piranha.Extend.Serializers;
using System;
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
        /// The currently registered media types.
        /// </summary>
        private MediaManager mediaTypes;

        /// <summary>
        /// The application object mapper.
        /// </summary>
        private IMapper mapper;

        /// <summary>
        /// The application markdown converter.
        /// </summary>
        private IMarkdown markdown;

        /// <summary>
        /// The currently registered serializers.
        /// </summary>
        private SerializerManager serializers;
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
        /// Gets the currently registered media types.
        /// </summary>
        public static MediaManager MediaTypes {
            get { return instance.mediaTypes; }
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
        public static IMarkdown Markdown {
            get { return instance.markdown; }
        }

        /// <summary>
        /// Gets the binding flags for retrieving a region from a
        /// strongly typed model.
        /// </summary>
        public static BindingFlags PropertyBindings {
            get { return BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance; }
        }

        /// <summary>
        /// Gets the currently registered serializers.
        /// </summary>
        public static SerializerManager Serializers {
            get { return instance.serializers; }
        }
        #endregion

        /// <summary>
        /// Default private constructor.
        /// </summary>
        private App() {
            fields = new AppFieldList();
            modules = new AppModuleList();
            mediaTypes = new MediaManager();
            serializers = new SerializerManager();
        }

        /// <summary>
        /// Initializes the application object.
        /// </summary>
        /// <param name="api">The current api</param>
        public static void Init(IApi api) {
            instance.Initialize(api);
        }

        public static string SerializeObject(object obj, Type type) {
            var serializer = instance.serializers[type];

            if (serializer != null)
                return serializer.Serialize(obj);
            return JsonConvert.SerializeObject(obj);
        }

        public static object DeserializeObject(string value, Type type) {
            var serializer = instance.serializers[type];
            
            if (serializer != null)
                return serializer.Deserialize(value);
            return JsonConvert.DeserializeObject(value, type);
        }

        #region Private methods
        /// <summary>
        /// Initializes the application object.
        /// </summary>
        /// <param name="api">The current api</param>
        private void Initialize(IApi api) {
            if (!isInitialized) {
                lock (mutex) {
                    if (!isInitialized) {
                        // Configure object mapper
                        var mapperConfig = new MapperConfiguration(cfg => {
                            cfg.CreateMap<Data.MediaFolder, Data.MediaFolder>()
                                .ForMember(f => f.Id, o => o.Ignore())
                                .ForMember(f => f.Created, o => o.Ignore())
                                .ForMember(f => f.Media, o => o.Ignore());
                            cfg.CreateMap<Data.MediaFolder, Models.MediaStructureItem>()
                                .ForMember(f => f.Level, o => o.Ignore())
                                .ForMember(f => f.Items, o => o.Ignore());                            
                            cfg.CreateMap<Data.Page, Models.PageBase>()
                                .ForMember(p => p.TypeId, o => o.MapFrom(m => m.PageTypeId));
                            cfg.CreateMap<Models.PageBase, Data.Page>()
                                .ForMember(p => p.PageTypeId, o => o.MapFrom(m => m.TypeId))
                                .ForMember(p => p.Fields, o => o.Ignore())
                                .ForMember(p => p.Created, o => o.Ignore())
                                .ForMember(p => p.LastModified, o => o.Ignore())
                                .ForMember(p => p.PageType, o => o.Ignore())
                                .ForMember(p => p.Site, o => o.Ignore())
                                .ForMember(p => p.Parent, o => o.Ignore());
                            cfg.CreateMap<Data.Page, Models.SitemapItem>()
                                .ForMember(p => p.MenuTitle, o => o.Ignore())
                                .ForMember(p => p.Level, o => o.Ignore())
                                .ForMember(p => p.Items, o => o.Ignore())
                                .ForMember(p => p.Permalink, o => o.MapFrom(d => !d.ParentId.HasValue && d.SortOrder == 0 ? "/" : "/" + d.Slug));
                            cfg.CreateMap<Data.Param, Data.Param>()
                                .ForMember(p => p.Id, o => o.Ignore())
                                .ForMember(p => p.Created, o => o.Ignore());
                            cfg.CreateMap<Data.Site, Data.Site>()
                                .ForMember(s => s.Id, o => o.Ignore())
                                .ForMember(s => s.Created, o => o.Ignore());
                        });
                        mapperConfig.AssertConfigurationIsValid();
                        mapper = mapperConfig.CreateMapper();

                        // Setup media types
                        mediaTypes.Documents.Add(".pdf", "application/pdf");
                        mediaTypes.Images.Add(".jpg", "image/jpeg");
                        mediaTypes.Images.Add(".jpeg", "image/jpeg");
                        mediaTypes.Images.Add(".png", "image/png");
                        mediaTypes.Videos.Add(".mp4", "video/mp4");

                        // Compose field types
                        fields.Register<Extend.Fields.DateField>();
                        fields.Register<Extend.Fields.HtmlField>();
                        fields.Register<Extend.Fields.ImageField>();
                        fields.Register<Extend.Fields.MarkdownField>();
                        fields.Register<Extend.Fields.StringField>();
                        fields.Register<Extend.Fields.TextField>();

                        // Compose serializers
                        serializers.Register<Extend.Fields.DateField>(new DateFieldSerializer());
                        serializers.Register<Extend.Fields.HtmlField>(new StringFieldSerializer<Extend.Fields.HtmlField>());
                        serializers.Register<Extend.Fields.MarkdownField>(new StringFieldSerializer<Extend.Fields.MarkdownField>());
                        serializers.Register<Extend.Fields.StringField>(new StringFieldSerializer<Extend.Fields.StringField>());
                        serializers.Register<Extend.Fields.TextField>(new StringFieldSerializer<Extend.Fields.TextField>());
                        serializers.Register<Extend.Fields.ImageField>(new ImageFieldSerializer());

                        // Create markdown converter
                        markdown = new DefaultMarkdown();

                        // Initialize all modules
                        foreach (var module in modules) {
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
