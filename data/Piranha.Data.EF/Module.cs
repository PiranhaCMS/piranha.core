/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System.Collections.Generic;
using AutoMapper;
using Piranha.Extend;
using Piranha.Security;

namespace Piranha.Data.EF
{
    /// <summary>
    /// The identity module.
    /// </summary>
    public class Module : IModule
    {
        public static IMapper Mapper { get; private set; }

        /// <summary>
        /// Gets the Author
        /// </summary>
        public string Author => "Piranha";

        /// <summary>
        /// Gets the Name
        /// </summary>
        public string Name => "Piranha.Data.EF";

        /// <summary>
        /// Gets the Version
        /// </summary>
        public string Version => Piranha.Utils.GetAssemblyVersion(GetType().Assembly);

        /// <summary>
        /// Gets the description
        /// </summary>
        public string Description => "Data implementation for Entity Framework Core.";

        /// <summary>
        /// Gets the package url.
        /// </summary>
        public string PackageUrl => "https://www.nuget.org/packages/Piranha.Data.EF";

        /// <summary>
        /// Gets the icon url.
        /// </summary>
        public string IconUrl => "http://piranhacms.org/assets/twitter-shield.png";

        /// <summary>
        /// Create automapping.
        /// </summary>
        static Module()
        {
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
                    .ForMember(p => p.ContentType, o => o.Ignore())
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
                    .ForMember(p => p.Permalink, o => o.Ignore())
                    .ForMember(p => p.Blocks, o => o.Ignore());
                cfg.CreateMap<Data.PostTag, Models.Taxonomy>()
                    .ForMember(p => p.Id, o => o.MapFrom(m => m.TagId))
                    .ForMember(p => p.Title, o => o.MapFrom(m => m.Tag.Title))
                    .ForMember(p => p.Slug, o => o.MapFrom(m => m.Tag.Slug));
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
            Mapper = mapperConfig.CreateMapper();
        }

        /// <summary>
        /// Initializes the module.
        /// </summary>
        public void Init() { }
    }
}