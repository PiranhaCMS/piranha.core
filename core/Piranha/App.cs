/*
 * Copyright (c) 2016-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using System.Reflection;
using AutoMapper;
using Newtonsoft.Json;
using Piranha.Data;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Extend.Serializers;
using Piranha.Models;
using PageField = Piranha.Extend.Fields.PageField;
using PostField = Piranha.Extend.Fields.PostField;

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
        private bool isInitialized;

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

        /// <summary>
        /// The currently registered content types.
        /// </summary>
        private ContentTypeManager contentTypes;

        /// <summary>
        /// The currently registered hooks.
        /// </summary>
        private HookManager hooks;
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

        /// <summary>
        /// Gets the currently registered content types.
        /// </summary>
        public static ContentTypeManager ContentTypes {
            get { return instance.contentTypes; }
        }

        /// <summary>
        /// Gets the currently registered hooks.
        /// </summary>
        public static HookManager Hooks {
            get { return instance.hooks; }
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
            contentTypes = new ContentTypeManager();
            hooks = new HookManager();
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
                            cfg.CreateMap<Alias, Alias>()
                                .ForMember(a => a.Id, o => o.Ignore())
                                .ForMember(a => a.Created, o => o.Ignore());
                            cfg.CreateMap<Category, Category>()
                                .ForMember(c => c.Id, o => o.Ignore())
                                .ForMember(c => c.Created, o => o.Ignore());
                            cfg.CreateMap<MediaFolder, MediaFolder>()
                                .ForMember(f => f.Id, o => o.Ignore())
                                .ForMember(f => f.Created, o => o.Ignore())
                                .ForMember(f => f.Media, o => o.Ignore());
                            cfg.CreateMap<MediaFolder, MediaStructureItem>()
                                .ForMember(f => f.Level, o => o.Ignore())
                                .ForMember(f => f.Items, o => o.Ignore());
                            cfg.CreateMap<Page, PageBase>()
                                .ForMember(p => p.TypeId, o => o.MapFrom(m => m.PageTypeId))
                                .ForMember(p => p.Permalink, o => o.MapFrom(m => "/" + m.Slug));
                            cfg.CreateMap<PageBase, Page>()
                                .ForMember(p => p.PageTypeId, o => o.MapFrom(m => m.TypeId))
                                .ForMember(p => p.Fields, o => o.Ignore())
                                .ForMember(p => p.Created, o => o.Ignore())
                                .ForMember(p => p.LastModified, o => o.Ignore())
                                .ForMember(p => p.PageType, o => o.Ignore())
                                .ForMember(p => p.Site, o => o.Ignore())
                                .ForMember(p => p.Parent, o => o.Ignore());
                            cfg.CreateMap<Page, SitemapItem>()
                                .ForMember(p => p.MenuTitle, o => o.Ignore())
                                .ForMember(p => p.Level, o => o.Ignore())
                                .ForMember(p => p.Items, o => o.Ignore())
                                .ForMember(p => p.PageTypeName, o => o.Ignore())
                                .ForMember(p => p.Permalink, o => o.MapFrom(d => !d.ParentId.HasValue && d.SortOrder == 0 ? "/" : "/" + d.Slug));
                            cfg.CreateMap<Param, Param>()
                                .ForMember(p => p.Id, o => o.Ignore())
                                .ForMember(p => p.Created, o => o.Ignore());
                            cfg.CreateMap<Post, PostBase>()
                                .ForMember(p => p.TypeId, o => o.MapFrom(m => m.PostTypeId))
                                .ForMember(p => p.Permalink, o => o.MapFrom(m => "/" + m.Blog.Slug + "/" + m.Slug));
                            cfg.CreateMap<PostBase, Post>()
                                .ForMember(p => p.PostTypeId, o => o.MapFrom(m => m.TypeId))
                                .ForMember(p => p.CategoryId, o => o.MapFrom(m => m.Category.Id))
                                .ForMember(p => p.Fields, o => o.Ignore())
                                .ForMember(p => p.Created, o => o.Ignore())
                                .ForMember(p => p.LastModified, o => o.Ignore())
                                .ForMember(p => p.PostType, o => o.Ignore())
                                .ForMember(p => p.Blog, o => o.Ignore())
                                .ForMember(p => p.Category, o => o.Ignore())
                                .ForMember(p => p.Tags, o => o.Ignore());
                            cfg.CreateMap<Site, Site>()
                                .ForMember(s => s.Id, o => o.Ignore())
                                .ForMember(s => s.Created, o => o.Ignore());
                            cfg.CreateMap<Tag, Tag>()
                                .ForMember(t => t.Id, o => o.Ignore())
                                .ForMember(t => t.Created, o => o.Ignore());
                        });
                        mapperConfig.AssertConfigurationIsValid();
                        mapper = mapperConfig.CreateMapper();

                        // Setup media types
                        mediaTypes.Documents.Add(".pdf", "application/pdf");
                        mediaTypes.Images.Add(".jpg", "image/jpeg");
                        mediaTypes.Images.Add(".jpeg", "image/jpeg");
                        mediaTypes.Images.Add(".png", "image/png");
                        mediaTypes.Videos.Add(".mp4", "video/mp4");

                        // Compose content types
                        contentTypes.Register<IPage>("Page", "Page");
                        contentTypes.Register<IBlogPage>("Blog", "Archive", true);                        

                        // Compose field types
                        fields.Register<DateField>();
                        fields.Register<DocumentField>();
                        fields.Register<HtmlField>();
                        fields.Register<ImageField>();
                        fields.Register<MarkdownField>();
                        fields.Register<MediaField>();
                        fields.Register<PageField>();
                        fields.Register<PostField>();
                        fields.Register<StringField>();
                        fields.Register<TextField>();
                        fields.Register<VideoField>();

                        // Compose serializers
                        serializers.Register<DateField>(new DateFieldSerializer());
                        serializers.Register<DocumentField>(new DocumentFieldSerializer());
                        serializers.Register<HtmlField>(new StringFieldSerializer<HtmlField>());
                        serializers.Register<MarkdownField>(new StringFieldSerializer<MarkdownField>());
                        serializers.Register<MediaField>(new MediaFieldSerializer());
                        serializers.Register<PageField>(new PageFieldSerializer());                        
                        serializers.Register<PostField>(new PostFieldSerializer());
                        serializers.Register<StringField>(new StringFieldSerializer<StringField>());
                        serializers.Register<TextField>(new StringFieldSerializer<TextField>());
                        serializers.Register<ImageField>(new ImageFieldSerializer());
                        serializers.Register<VideoField>(new VideoFieldSerializer());

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
