/*
 * Copyright (c) 2016-2018 Håkan Edling
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
using Piranha.Runtime;
using Piranha.Security;
using System;
using System.Reflection;

namespace Piranha
{
    /// <summary>
    /// The main application object.
    /// </summary>
    public sealed class App
    {
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
        private bool _isInitialized = false;

        /// <summary>
        /// The currently registered blocks.
        /// </summary>
        private AppBlockList _blocks;

        /// <summary>
        /// The currently registered fields.
        /// </summary>
        private AppFieldList _fields;

        /// <summary>
        /// The currently registered modules.
        /// </summary>
        private AppModuleList _modules;

        /// <summary>
        /// The currently registered media types.
        /// </summary>
        private MediaManager _mediaTypes;

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
        private SerializerManager _serializers;

        /// <summary>
        /// The currently registered content types.
        /// </summary>
        private ContentTypeManager _contentTypes;

        /// <summary>
        /// The currently registered hooks.
        /// </summary>
        private HookManager _hooks;

        /// <summary>
        /// The currently registered permissions;
        /// </summary>
        private PermissionManager _permissions;

        /// <summary>
        /// The current cache level.
        /// </summary>
        private Cache.CacheLevel _cacheLevel = Cache.CacheLevel.Full;

        /// <summary>
        /// Gets the currently registered block types.
        /// </summary>
        public static AppBlockList Blocks => Instance._blocks;

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
        public static BindingFlags PropertyBindings =>
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance;

        /// <summary>
        /// Gets the currently registered serializers.
        /// </summary>
        public static SerializerManager Serializers => Instance._serializers;

        /// <summary>
        /// Gets the currently registered content types.
        /// </summary>
        public static ContentTypeManager ContentTypes => Instance._contentTypes;

        /// <summary>
        /// Gets the currently registered hooks.
        /// </summary>
        public static HookManager Hooks => Instance._hooks;

        /// <summary>
        /// Gets the currently registered permissions.
        /// </summary>
        public static PermissionManager Permissions => Instance._permissions;

        /// <summary>
        /// Gets/sets the current cache level.
        /// </summary>
        public static Cache.CacheLevel CacheLevel
        {
            get { return Instance._cacheLevel; }
            set { Instance._cacheLevel = value; }
        }

        /// <summary>
        /// Default private constructor.
        /// </summary>
        private App()
        {
            _blocks = new AppBlockList();
            _fields = new AppFieldList();
            _modules = new AppModuleList();
            _mediaTypes = new MediaManager();
            _serializers = new SerializerManager();
            _contentTypes = new ContentTypeManager();
            _hooks = new HookManager();
            _permissions = new PermissionManager();
        }

        /// <summary>
        /// Initializes the application object.
        /// </summary>
        public static void Init()
        {
            Instance.Initialize();
        }

        /// <summary>
        /// Initializes the application object.
        /// </summary>
        [Obsolete("Please refer to App.Init()")]
        public static void Init(IApi api)
        {
            Instance.Initialize();
        }

        /// <summary>
        /// Serializes the given object.
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <param name="type">The type</param>
        /// <returns>The serialized object</returns>
        public static string SerializeObject(object obj, Type type)
        {
            var serializer = Instance._serializers[type];

            if (serializer != null)
                return serializer.Serialize(obj);
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// Deserializes the given value.
        /// </summary>
        /// <param name="value">The serialized value</param>
        /// <param name="type">The type</param>
        /// <returns>The deserialized object</returns>
        public static object DeserializeObject(string value, Type type)
        {
            var serializer = Instance._serializers[type];

            if (serializer != null)
                return serializer.Deserialize(value);
            return JsonConvert.DeserializeObject(value, type);
        }

        /// <summary>
        /// Initializes the application object.
        /// </summary>
        /// <param name="api">The current api</param>
        private void Initialize()
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
                            cfg.CreateMap<Data.Alias, Data.Alias>()
                                .ForMember(a => a.Id, o => o.Ignore())
                                .ForMember(a => a.Created, o => o.Ignore());
                            cfg.CreateMap<Data.Category, Data.Category>()
                                .ForMember(c => c.Id, o => o.Ignore())
                                .ForMember(c => c.Created, o => o.Ignore());
                            cfg.CreateMap<Data.MediaFolder, Data.MediaFolder>()
                                .ForMember(f => f.Id, o => o.Ignore())
                                .ForMember(f => f.Created, o => o.Ignore())
                                .ForMember(f => f.Media, o => o.Ignore());
                            cfg.CreateMap<Data.MediaFolder, Models.MediaStructureItem>()
                                .ForMember(f => f.Level, o => o.Ignore())
                                .ForMember(f => f.Items, o => o.Ignore());
                            cfg.CreateMap<Data.Page, Models.PageBase>()
                                .ForMember(p => p.TypeId, o => o.MapFrom(m => m.PageTypeId))
                                .ForMember(p => p.Permalink, o => o.MapFrom(m => "/" + m.Slug))
                                .ForMember(p => p.Blocks, o => o.Ignore());
                            cfg.CreateMap<Models.PageBase, Data.Page>()
                                .ForMember(p => p.PageTypeId, o => o.MapFrom(m => m.TypeId))
                                .ForMember(p => p.Blocks, o => o.Ignore())
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
                                .ForMember(p => p.PageTypeName, o => o.Ignore())
                                .ForMember(p => p.Permalink, o => o.MapFrom(d => !d.ParentId.HasValue && d.SortOrder == 0 ? "/" : "/" + d.Slug));
                            cfg.CreateMap<Data.Param, Data.Param>()
                                .ForMember(p => p.Id, o => o.Ignore())
                                .ForMember(p => p.Created, o => o.Ignore());
                            cfg.CreateMap<Data.Post, Models.PostBase>()
                                .ForMember(p => p.TypeId, o => o.MapFrom(m => m.PostTypeId))
                                .ForMember(p => p.Permalink, o => o.MapFrom(m => "/" + m.Blog.Slug + "/" + m.Slug))
                                .ForMember(p => p.Blocks, o => o.Ignore());
                            cfg.CreateMap<Models.PostBase, Data.Post>()
                                .ForMember(p => p.PostTypeId, o => o.MapFrom(m => m.TypeId))
                                .ForMember(p => p.CategoryId, o => o.MapFrom(m => m.Category.Id))
                                .ForMember(p => p.Blocks, o => o.Ignore())
                                .ForMember(p => p.Fields, o => o.Ignore())
                                .ForMember(p => p.Created, o => o.Ignore())
                                .ForMember(p => p.LastModified, o => o.Ignore())
                                .ForMember(p => p.PostType, o => o.Ignore())
                                .ForMember(p => p.Blog, o => o.Ignore())
                                .ForMember(p => p.Category, o => o.Ignore())
                                .ForMember(p => p.Tags, o => o.Ignore());
                            cfg.CreateMap<Data.Site, Data.Site>()
                                .ForMember(s => s.Id, o => o.Ignore())
                                .ForMember(s => s.Created, o => o.Ignore());
                            cfg.CreateMap<Data.Site, Models.SiteContentBase>()
                                .ForMember(s => s.TypeId, o => o.MapFrom(m => m.SiteTypeId));
                            cfg.CreateMap<Models.SiteContentBase, Data.Site>()
                                .ForMember(s => s.SiteTypeId, o => o.Ignore())
                                .ForMember(s => s.InternalId, o => o.Ignore())
                                .ForMember(s => s.Description, o => o.Ignore())
                                .ForMember(s => s.Hostnames, o => o.Ignore())
                                .ForMember(s => s.IsDefault, o => o.Ignore())
                                .ForMember(s => s.Culture, o => o.Ignore())
                                .ForMember(s => s.Fields, o => o.Ignore())
                                .ForMember(s => s.Created, o => o.Ignore())
                                .ForMember(s => s.LastModified, o => o.Ignore())
                                .ForMember(s => s.ContentLastModified, o => o.Ignore());
                            cfg.CreateMap<Data.Tag, Data.Tag>()
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
                        _contentTypes.Register<Models.IPage>("Page", "Page");
                        _contentTypes.Register<Models.IArchivePage>("Blog", "Archive", true);

                        // Compose field types
                        _fields.Register<Extend.Fields.CheckBoxField>();
                        _fields.Register<Extend.Fields.DateField>();
                        _fields.Register<Extend.Fields.DocumentField>();
                        _fields.Register<Extend.Fields.HtmlField>();
                        _fields.Register<Extend.Fields.ImageField>();
                        _fields.Register<Extend.Fields.MarkdownField>();
                        _fields.Register<Extend.Fields.MediaField>();
                        _fields.Register<Extend.Fields.NumberField>();
                        _fields.Register<Extend.Fields.PageField>();
                        _fields.Register<Extend.Fields.PostField>();
                        _fields.Register<Extend.Fields.StringField>();
                        _fields.Register<Extend.Fields.TextField>();
                        _fields.Register<Extend.Fields.VideoField>();

                        // Compose block types
                        _blocks.Register<Extend.Blocks.HtmlBlock>();
                        _blocks.Register<Extend.Blocks.HtmlColumnBlock>();
                        _blocks.Register<Extend.Blocks.ImageBlock>();
                        _blocks.Register<Extend.Blocks.QuoteBlock>();
                        _blocks.Register<Extend.Blocks.TextBlock>();

                        // Compose serializers
                        _serializers.Register<Extend.Fields.CheckBoxField>(new CheckBoxFieldSerializer<Extend.Fields.CheckBoxField>());
                        _serializers.Register<Extend.Fields.DateField>(new DateFieldSerializer());
                        _serializers.Register<Extend.Fields.DocumentField>(new DocumentFieldSerializer());
                        _serializers.Register<Extend.Fields.HtmlField>(new StringFieldSerializer<Extend.Fields.HtmlField>());
                        _serializers.Register<Extend.Fields.MarkdownField>(new StringFieldSerializer<Extend.Fields.MarkdownField>());
                        _serializers.Register<Extend.Fields.MediaField>(new MediaFieldSerializer());
                        _serializers.Register<Extend.Fields.NumberField>(new IntegerFieldSerializer<Extend.Fields.NumberField>());
                        _serializers.Register<Extend.Fields.PageField>(new PageFieldSerializer());
                        _serializers.Register<Extend.Fields.PostField>(new PostFieldSerializer());
                        _serializers.Register<Extend.Fields.StringField>(new StringFieldSerializer<Extend.Fields.StringField>());
                        _serializers.Register<Extend.Fields.TextField>(new StringFieldSerializer<Extend.Fields.TextField>());
                        _serializers.Register<Extend.Fields.ImageField>(new ImageFieldSerializer());
                        _serializers.Register<Extend.Fields.VideoField>(new VideoFieldSerializer());

                        // Create markdown converter
                        _markdown = new DefaultMarkdown();

                        // Register permissions
                        _permissions["Core"].Add(new PermissionItem
                        {
                            Name = Permission.PagePreview,
                            Title = "Page Preview"
                        });
                        _permissions["Core"].Add(new PermissionItem
                        {
                            Name = Permission.PostPreview,
                            Title = "Post Preview"
                        });

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
    }
}
