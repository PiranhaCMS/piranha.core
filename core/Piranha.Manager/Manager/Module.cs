/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using System.IO;
using System.Reflection;
using AutoMapper;
using Piranha.Areas.Manager.Models;
using Piranha.Extend;
using Piranha.Models;

namespace Piranha.Manager
{
    public class Module : IModule
    {
        #region Properties
        /// <summary>
        /// Gets the mapper.
        /// </summary>
        public static IMapper Mapper { get; private set; }

        /// <summary>
        /// The assembly.
        /// </summary>
        internal static Assembly Assembly;

        /// <summary>
        /// Last modification date of the assembly.
        /// </summary>
        internal static DateTime LastModified;
        #endregion

        /// <summary>
        /// Initializes the module.
        /// </summary>
        public void Init() {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<PageBase, PageEditModel>()
                    .ForMember(m => m.PageType, o => o.Ignore())
                    .ForMember(m => m.Regions, o => o.Ignore())
                    .ForMember(m => m.PageContentType, o => o.Ignore());
                cfg.CreateMap<PageEditModel, PageBase>()
                    .ForMember(m => m.TypeId, o => o.Ignore())
                    .ForMember(m => m.Created, o => o.Ignore())
                    .ForMember(m => m.LastModified, o => o.Ignore());
                cfg.CreateMap<PostBase, PostEditModel>()
                    .ForMember(m => m.PostType, o => o.Ignore())
                    .ForMember(m => m.Regions, o => o.Ignore())
                    .ForMember(m => m.AllCategories, o => o.Ignore())
                    .ForMember(m => m.AllTags, o => o.Ignore())
                    .ForMember(m => m.SelectedCategory, o => o.Ignore())
                    .ForMember(m => m.SelectedTags, o => o.Ignore())
                    .ForMember(m => m.BlogSlug, o => o.Ignore());
                cfg.CreateMap<PostEditModel, PostBase>()
                    .ForMember(m => m.TypeId, o => o.Ignore())
                    .ForMember(m => m.Created, o => o.Ignore())
                    .ForMember(m => m.LastModified, o => o.Ignore());
            });

            config.AssertConfigurationIsValid();
            Mapper = config.CreateMapper();

            // Get assembly information
            Assembly = GetType().GetTypeInfo().Assembly;
            LastModified = new FileInfo(Assembly.Location).LastWriteTime;
        }
    }
}
