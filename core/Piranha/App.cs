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
        private static readonly App Instance = new App();

        /// <summary>
        /// Mutex for thread safe initialization.
        /// </summary>
        private readonly object _mutex = new object();

        /// <summary>
        /// The current state of the app.
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        /// The currently registered fields.
        /// </summary>
        private readonly AppFieldList _fields;

        /// <summary>
        /// The currently registered modules.
        /// </summary>
        private readonly AppModuleList _modules;

        /// <summary>
        /// The currently registered media types.
        /// </summary>
        private readonly MediaManager _mediaTypes;

        /// <summary>
        /// The application object mapper.
        /// </summary>
        private IMapper _mapper;

        /// <summary>
        /// The application markdown converter.
        /// </summary>
        private IMarkdown _markdown;

        /// <summary>
        /// The currently registered serializers.
        /// </summary>
        private readonly SerializerManager _serializers;

        /// <summary>
        /// The currently registered content types.
        /// </summary>
        private readonly ContentTypeManager contentTypes;

        /// <summary>
        /// The currently registered hooks.
        /// </summary>
        private readonly HookManager _hooks;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the currently registered field types.
        /// </summary>
        public static AppFieldList Fields => Instance._fields;

        /// <summary>
        /// Gets the currently registred modules.
        /// </summary>
        public static AppModuleList Modules => Instance._modules;

        /// <summary>
        /// Gets the currently registered media types.
        /// </summary>
        public static MediaManager MediaTypes => Instance._mediaTypes;

        /// <summary>
        /// Gets the application object mapper.
        /// </summary>
        public static IMapper Mapper => Instance._mapper;

        /// <summary>
        /// Gets the markdown converter.
        /// </summary>
        public static IMarkdown Markdown => Instance._markdown;

        /// <summary>
        /// Gets the binding flags for retrieving a region from a
        /// strongly typed model.
        /// </summary>
        public static BindingFlags PropertyBindings => BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance;

        /// <summary>
        /// Gets the currently registered serializers.
        /// </summary>
        public static SerializerManager Serializers => Instance._serializers;

        /// <summary>
        /// Gets the currently registered content types.
        /// </summary>
        public static ContentTypeManager ContentTypes
        {
            get { return Instance.contentTypes; }
        }

        /// <summary>
        /// Gets the currently registered hooks.
        /// </summary>
        public static HookManager Hooks => Instance._hooks;

        #endregion

        /// <summary>
        /// Default private constructor.
        /// </summary>
        private App()
        {
            _fields = new AppFieldList();
            _modules = new AppModuleList();
            _mediaTypes = new MediaManager();
            _serializers = new SerializerManager();
            contentTypes = new ContentTypeManager();
            _hooks = new HookManager();
        }

        /// <summary>
        /// Initializes the application object.
        /// </summary>
        /// <param name="api">The current api</param>
        public static void Init(IApi api)
        {
            Instance.Initialize(api);
        }

        public static string SerializeObject(object obj, Type type)
        {
            var serializer = Instance._serializers[type];

            return serializer != null ? serializer.Serialize(obj) : JsonConvert.SerializeObject(obj);
        }

        public static object DeserializeObject(string value, Type type)
        {
            var serializer = Instance._serializers[type];

            return serializer != null ? serializer.Deserialize(value) : JsonConvert.DeserializeObject(value, type);
        }

        #region Private methods
        /// <summary>
        /// Initializes the application object.
        /// </summary>
        /// <param name="api">The current api</param>
        private void Initialize(IApi api)
        {
            if (!_isInitialized)
            {
                lock (_mutex)
                {
                    if (!_isInitialized)
                    {
                        // Configure object mapper
                        var mapperConfig = new MapperConfiguration(cfg =>
                        {
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
                        _mapper = mapperConfig.CreateMapper();

                        // Setup media types
                        _mediaTypes.Documents.Add(".pdf", "application/pdf");
                        _mediaTypes.Images.Add(".jpg", "image/jpeg");
                        _mediaTypes.Images.Add(".jpeg", "image/jpeg");
                        _mediaTypes.Images.Add(".png", "image/png");
                        _mediaTypes.Videos.Add(".mp4", "video/mp4");

                        // Compose content types
                        contentTypes.Register<IPage>("Page", "Page");
                        contentTypes.Register<IBlogPage>("Blog", "Archive", true);

                        // Compose field types
                        _fields.Register<DateField>();
                        _fields.Register<DocumentField>();
                        _fields.Register<HtmlField>();
                        _fields.Register<ImageField>();
                        _fields.Register<MarkdownField>();
                        _fields.Register<MediaField>();
                        _fields.Register<PageField>();
                        _fields.Register<PostField>();
                        _fields.Register<StringField>();
                        _fields.Register<TextField>();
                        _fields.Register<VideoField>();

                        // Compose serializers
                        _serializers.Register<DateField>(new DateFieldSerializer());
                        _serializers.Register<DocumentField>(new DocumentFieldSerializer());
                        _serializers.Register<HtmlField>(new StringFieldSerializer<HtmlField>());
                        _serializers.Register<MarkdownField>(new StringFieldSerializer<MarkdownField>());
                        _serializers.Register<MediaField>(new MediaFieldSerializer());
                        _serializers.Register<PageField>(new PageFieldSerializer());
                        _serializers.Register<PostField>(new PostFieldSerializer());
                        _serializers.Register<StringField>(new StringFieldSerializer<StringField>());
                        _serializers.Register<TextField>(new StringFieldSerializer<TextField>());
                        _serializers.Register<ImageField>(new ImageFieldSerializer());
                        _serializers.Register<VideoField>(new VideoFieldSerializer());

                        // Create markdown converter
                        _markdown = new DefaultMarkdown();

                        // Initialize all modules
                        foreach (var module in _modules)
                        {
                            module.Instance.Init();
                        }

                        _isInitialized = true;
                    }
                }
            }
        }
        #endregion
    }
}
