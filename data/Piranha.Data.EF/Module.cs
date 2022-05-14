/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using AutoMapper;
using Piranha.Extend;

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
        public string IconUrl => "https://piranhacms.org/assets/twitter-shield.png";

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
                cfg.CreateMap<Data.Category, Models.Taxonomy>()
                    .ForMember(c => c.Type, o => o.MapFrom(m => Models.TaxonomyType.Category));
                cfg.CreateMap<Data.Content, Models.GenericContent>()
                    .ForMember(p => p.PrimaryImage, o => o.MapFrom(m => m.PrimaryImageId))
                    .ForMember(p => p.Permissions, o => o.Ignore());
                cfg.CreateMap<Models.GenericContent, Data.Content>()
                    .ForMember(c => c.CategoryId, o => o.Ignore())
                    .ForMember(c => c.Category, o => o.Ignore())
                    .ForMember(c => c.Blocks, o => o.Ignore())
                    .ForMember(c => c.Fields, o => o.Ignore())
                    .ForMember(c => c.Tags, o => o.Ignore())
                    .ForMember(c => c.Type, o => o.Ignore())
                    .ForMember(c => c.Translations, o => o.Ignore())
                    .ForMember(c => c.Created, o => o.Ignore())
                    .ForMember(c => c.LastModified, o => o.Ignore());
                cfg.CreateMap<Data.ContentGroup, Models.ContentGroup>();
                cfg.CreateMap<Models.ContentGroup, Data.ContentGroup>()
                    .ForMember(g => g.Created, o => o.Ignore())
                    .ForMember(g => g.LastModified, o => o.Ignore());
                cfg.CreateMap<Data.ContentTranslation, Models.GenericContent>()
                    .ForMember(c =>  c.Id, o => o.Ignore())
                    .ForMember(c =>  c.TypeId, o => o.Ignore())
                    .ForMember(c =>  c.PrimaryImage, o => o.Ignore())
                    .ForMember(c =>  c.Created, o => o.Ignore())
                    .ForMember(c =>  c.LastModified, o => o.Ignore())
                    .ForMember(c =>  c.Permissions, o => o.Ignore());
                cfg.CreateMap<Data.MediaFolder, Data.MediaFolder>()
                    .ForMember(f => f.Id, o => o.Ignore())
                    .ForMember(f => f.Created, o => o.Ignore())
                    .ForMember(f => f.Media, o => o.Ignore());
                cfg.CreateMap<Data.MediaFolder, Models.MediaStructureItem>()
                    .ForMember(f => f.Level, o => o.Ignore())
                    .ForMember(f => f.FolderCount, o => o.Ignore())
                    .ForMember(f => f.MediaCount, o => o.Ignore())
                    .ForMember(f => f.Items, o => o.Ignore());
                cfg.CreateMap<Data.Page, Models.PageBase>()
                    .ForMember(p => p.TypeId, o => o.MapFrom(m => m.PageTypeId))
                    .ForMember(p => p.PrimaryImage, o => o.MapFrom(m => m.PrimaryImageId))
                    .ForMember(p => p.OgImage, o => o.MapFrom(m => m.OgImageId))
                    .ForMember(p => p.Permalink, o => o.MapFrom(m => "/" + m.Slug))
                    .ForMember(p => p.Permissions, o => o.Ignore())
                    .ForMember(p => p.Blocks, o => o.Ignore())
                    .ForMember(p => p.CommentCount, o => o.Ignore());
                cfg.CreateMap<Models.PageBase, Data.Page>()
                    .ForMember(p => p.ContentType, o => o.Ignore())
                    .ForMember(p => p.PrimaryImageId, o => o.MapFrom(m => m.PrimaryImage != null ? m.PrimaryImage.Id : (Guid?)null ))
                    .ForMember(p => p.OgImageId, o => o.MapFrom(m => m.OgImage != null ? m.OgImage.Id : (Guid?)null ))
                    .ForMember(p => p.PageTypeId, o => o.MapFrom(m => m.TypeId))
                    .ForMember(p => p.Blocks, o => o.Ignore())
                    .ForMember(p => p.Fields, o => o.Ignore())
                    .ForMember(p => p.Created, o => o.Ignore())
                    .ForMember(p => p.LastModified, o => o.Ignore())
                    .ForMember(p => p.Permissions, o => o.Ignore())
                    .ForMember(p => p.PageType, o => o.Ignore())
                    .ForMember(p => p.Site, o => o.Ignore())
                    .ForMember(p => p.Parent, o => o.Ignore());
                cfg.CreateMap<Data.Page, Models.SitemapItem>()
                    .ForMember(p => p.MenuTitle, o => o.Ignore())
                    .ForMember(p => p.Level, o => o.Ignore())
                    .ForMember(p => p.Items, o => o.Ignore())
                    .ForMember(p => p.PageTypeName, o => o.Ignore())
                    .ForMember(p => p.Permalink, o => o.MapFrom(d => !d.ParentId.HasValue && d.SortOrder == 0 ? "/" : "/" + d.Slug))
                    .ForMember(p => p.Permissions, o => o.MapFrom(d => d.Permissions.Select(dp => dp.Permission).ToList()));
                cfg.CreateMap<Data.Param, Data.Param>()
                    .ForMember(p => p.Id, o => o.Ignore())
                    .ForMember(p => p.Created, o => o.Ignore());
                cfg.CreateMap<Data.Post, Models.PostBase>()
                    .ForMember(p => p.TypeId, o => o.MapFrom(m => m.PostTypeId))
                    .ForMember(p => p.PrimaryImage, o => o.MapFrom(m => m.PrimaryImageId))
                    .ForMember(p => p.OgImage, o => o.MapFrom(m => m.OgImageId))
                    .ForMember(p => p.Permalink, o => o.Ignore())
                    .ForMember(p => p.Permissions, o => o.Ignore())
                    .ForMember(p => p.Blocks, o => o.Ignore())
                    .ForMember(p => p.CommentCount, o => o.Ignore());
                cfg.CreateMap<Data.PostTag, Models.Taxonomy>()
                    .ForMember(p => p.Id, o => o.MapFrom(m => m.TagId))
                    .ForMember(p => p.Title, o => o.MapFrom(m => m.Tag.Title))
                    .ForMember(p => p.Slug, o => o.MapFrom(m => m.Tag.Slug))
                    .ForMember(p => p.Type, o => o.MapFrom(m => Models.TaxonomyType.Tag));
                cfg.CreateMap<Models.PostBase, Data.Post>()
                    .ForMember(p => p.PostTypeId, o => o.MapFrom(m => m.TypeId))
                    .ForMember(p => p.CategoryId, o => o.MapFrom(m => m.Category.Id))
                    .ForMember(p => p.PrimaryImageId, o => o.MapFrom(m => m.PrimaryImage != null ? m.PrimaryImage.Id : (Guid?)null ))
                    .ForMember(p => p.OgImageId, o => o.MapFrom(m => m.OgImage != null ? m.OgImage.Id : (Guid?)null ))
                    .ForMember(p => p.Blocks, o => o.Ignore())
                    .ForMember(p => p.Fields, o => o.Ignore())
                    .ForMember(p => p.Created, o => o.Ignore())
                    .ForMember(p => p.LastModified, o => o.Ignore())
                    .ForMember(p => p.Permissions, o => o.Ignore())
                    .ForMember(p => p.PostType, o => o.Ignore())
                    .ForMember(p => p.Blog, o => o.Ignore())
                    .ForMember(p => p.Category, o => o.Ignore())
                    .ForMember(p => p.Tags, o => o.Ignore());
                cfg.CreateMap<Data.Site, Data.Site>()
                    .ForMember(s => s.Id, o => o.Ignore())
                    .ForMember(s => s.Language, o => o.Ignore())
                    .ForMember(s => s.Created, o => o.Ignore());
                cfg.CreateMap<Data.Site, Models.SiteContentBase>()
                    .ForMember(s => s.TypeId, o => o.MapFrom(m => m.SiteTypeId))
                    .ForMember(s => s.Permissions, o => o.Ignore());
                cfg.CreateMap<Models.SiteContentBase, Data.Site>()
                    .ForMember(s => s.LanguageId, o => o.Ignore())
                    .ForMember(s => s.SiteTypeId, o => o.Ignore())
                    .ForMember(s => s.InternalId, o => o.Ignore())
                    .ForMember(s => s.Description, o => o.Ignore())
                    .ForMember(s => s.LogoId, o => o.Ignore())
                    .ForMember(s => s.Hostnames, o => o.Ignore())
                    .ForMember(s => s.IsDefault, o => o.Ignore())
                    .ForMember(s => s.Culture, o => o.Ignore())
                    .ForMember(s => s.Fields, o => o.Ignore())
                    .ForMember(s => s.Language, o => o.Ignore())
                    .ForMember(s => s.Created, o => o.Ignore())
                    .ForMember(s => s.LastModified, o => o.Ignore())
                    .ForMember(s => s.ContentLastModified, o => o.Ignore());
                cfg.CreateMap<Data.Tag, Data.Tag>()
                    .ForMember(t => t.Id, o => o.Ignore())
                    .ForMember(t => t.Created, o => o.Ignore());
                cfg.CreateMap<Data.Tag, Models.Taxonomy>()
                    .ForMember(t => t.Type, o => o.MapFrom(m => Models.TaxonomyType.Tag));
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