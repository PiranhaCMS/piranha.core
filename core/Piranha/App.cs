/*
 * Copyright (c) 2016-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Reflection;
using Newtonsoft.Json;
using Piranha.Extend;
using Piranha.Extend.Serializers;
using Piranha.Runtime;
using Piranha.Security;

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
        private static readonly App Instance;

        /// <summary>
        /// Mutex for thread safe initialization.
        /// </summary>
        private static readonly object _mutex = new object();

        /// <summary>
        /// If the app has been initialized.
        /// </summary>
        private static volatile bool _isInitialized = false;

        /// <summary>
        /// The currently registered blocks.
        /// </summary>
        private readonly AppBlockList _blocks;

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
        /// The currently registered serializers.
        /// </summary>
        private readonly SerializerManager _serializers;

        /// <summary>
        /// The currently registered hooks.
        /// </summary>
        private readonly HookManager _hooks;

        /// <summary>
        /// The currently registered permissions;
        /// </summary>
        private readonly PermissionManager _permissions;

        /// <summary>
        /// The current cache level.
        /// </summary>
        private Cache.CacheLevel _cacheLevel = Cache.CacheLevel.Full;

        /// <summary>
        /// The application markdown converter.
        /// </summary>
        private IMarkdown _markdown;

        /// <summary>
        /// The currently available page types.
        /// </summary>
        private readonly ContentTypeList<Models.PageType> _pageTypes;

        /// <summary>
        /// The currently available post types.
        /// </summary>
        private readonly ContentTypeList<Models.PostType> _postTypes;

        /// <summary>
        /// The currently available post types.
        /// </summary>
        private readonly ContentTypeList<Models.SiteType> _siteTypes;

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
        /// Gets the currently available page types.
        /// </summary>
        public static ContentTypeList<Models.PageType> PageTypes => Instance._pageTypes;

        /// <summary>
        /// Gets the currently available page types.
        /// </summary>
        public static ContentTypeList<Models.PostType> PostTypes => Instance._postTypes;

        /// <summary>
        /// Gets the currently available page types.
        /// </summary>
        public static ContentTypeList<Models.SiteType> SiteTypes => Instance._siteTypes;

        /// <summary>
        /// Static constructor. Called once every application
        /// lifecycle.
        /// </summary>
        static App()
        {
            Instance = new App();

            // Setup media types
            Instance._mediaTypes.Documents.Add(".pdf", "application/pdf");
            Instance._mediaTypes.Images.Add(".jpg", "image/jpeg");
            Instance._mediaTypes.Images.Add(".jpeg", "image/jpeg");
            Instance._mediaTypes.Images.Add(".png", "image/png");
            Instance._mediaTypes.Videos.Add(".mp4", "video/mp4");
            Instance._mediaTypes.Audio.Add(".mp3", "audio/mpeg");
            Instance._mediaTypes.Audio.Add(".wav", "audio/wav");

            // Compose field types
            Instance._fields.Register<Extend.Fields.CheckBoxField>();
            Instance._fields.Register<Extend.Fields.DateField>();
            Instance._fields.Register<Extend.Fields.DocumentField>();
            Instance._fields.Register<Extend.Fields.HtmlField>();
            Instance._fields.Register<Extend.Fields.ImageField>();
            Instance._fields.Register<Extend.Fields.MarkdownField>();
            Instance._fields.Register<Extend.Fields.MediaField>();
            Instance._fields.Register<Extend.Fields.NumberField>();
            Instance._fields.Register<Extend.Fields.PageField>();
            Instance._fields.Register<Extend.Fields.PostField>();
            Instance._fields.Register<Extend.Fields.ReadonlyField>();
            Instance._fields.Register<Extend.Fields.StringField>();
            Instance._fields.Register<Extend.Fields.TextField>();
            Instance._fields.Register<Extend.Fields.VideoField>();
            Instance._fields.Register<Extend.Fields.AudioField>();

            // Compose block types
            Instance._blocks.Register<Extend.Blocks.AudioBlock>();
            Instance._blocks.Register<Extend.Blocks.ColumnBlock>();
            Instance._blocks.Register<Extend.Blocks.HtmlBlock>();
            Instance._blocks.Register<Extend.Blocks.HtmlColumnBlock>();
            Instance._blocks.Register<Extend.Blocks.ImageBlock>();
            Instance._blocks.Register<Extend.Blocks.ImageGalleryBlock>();
            Instance._blocks.Register<Extend.Blocks.QuoteBlock>();
            Instance._blocks.Register<Extend.Blocks.SeparatorBlock>();
            Instance._blocks.Register<Extend.Blocks.TextBlock>();
            Instance._blocks.Register<Extend.Blocks.VideoBlock>();

            // Compose serializers
            Instance._serializers.Register<Extend.Fields.CheckBoxField>(new CheckBoxFieldSerializer<Extend.Fields.CheckBoxField>());
            Instance._serializers.Register<Extend.Fields.DateField>(new DateFieldSerializer());
            Instance._serializers.Register<Extend.Fields.DocumentField>(new DocumentFieldSerializer());
            Instance._serializers.Register<Extend.Fields.HtmlField>(new StringFieldSerializer<Extend.Fields.HtmlField>());
            Instance._serializers.Register<Extend.Fields.MarkdownField>(new StringFieldSerializer<Extend.Fields.MarkdownField>());
            Instance._serializers.Register<Extend.Fields.MediaField>(new MediaFieldSerializer());
            Instance._serializers.Register<Extend.Fields.NumberField>(new IntegerFieldSerializer<Extend.Fields.NumberField>());
            Instance._serializers.Register<Extend.Fields.PageField>(new PageFieldSerializer());
            Instance._serializers.Register<Extend.Fields.PostField>(new PostFieldSerializer());
            Instance._serializers.Register<Extend.Fields.StringField>(new StringFieldSerializer<Extend.Fields.StringField>());
            Instance._serializers.Register<Extend.Fields.TextField>(new StringFieldSerializer<Extend.Fields.TextField>());
            Instance._serializers.Register<Extend.Fields.ImageField>(new ImageFieldSerializer());
            Instance._serializers.Register<Extend.Fields.VideoField>(new VideoFieldSerializer());
            Instance._serializers.Register<Extend.Fields.AudioField>(new AudioFieldSerializer());

            // Create markdown converter
            Instance._markdown = new DefaultMarkdown();

            // Register permissions
            Instance._permissions["Core"].Add(new PermissionItem
            {
                Name = Permission.PagePreview,
                Title = "Page Preview"
            });
            Instance._permissions["Core"].Add(new PermissionItem
            {
                Name = Permission.PostPreview,
                Title = "Post Preview"
            });
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
            _hooks = new HookManager();
            _permissions = new PermissionManager();
            _pageTypes = new ContentTypeList<Models.PageType>();
            _postTypes = new ContentTypeList<Models.PostType>();
            _siteTypes = new ContentTypeList<Models.SiteType>();
        }

        /// <summary>
        /// Initializes the application.
        /// </summary>
        public static void Init(IApi api)
        {
            Instance.InitApp(api);
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
        private void InitApp(IApi api)
        {
            if (!_isInitialized)
            {
                lock (_mutex)
                {
                    if (!_isInitialized)
                    {
                        // Initialize content types
                        _pageTypes.Init(api.PageTypes.GetAllAsync().GetAwaiter().GetResult());
                        _postTypes.Init(api.PostTypes.GetAllAsync().GetAwaiter().GetResult());
                        _siteTypes.Init(api.SiteTypes.GetAllAsync().GetAwaiter().GetResult());

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
