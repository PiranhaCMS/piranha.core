/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using AutoMapper;

namespace Piranha.Manager
{
    public class Module : Extend.IModule
    {
        /// <summary>
        /// Gets the mapper.
        /// </summary>
        public static IMapper Mapper { get; private set; }

        /// <summary>
        /// Initializes the module.
        /// </summary>
        public void Init() {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Models.PageModelBase, Areas.Manager.Models.PageEditModel>()
                    .ForMember(m => m.PageType, o => o.Ignore())
                    .ForMember(m => m.Regions, o => o.Ignore());
                cfg.CreateMap<Areas.Manager.Models.PageEditModel, Models.PageModelBase>()
                    .ForMember(m => m.TypeId, o => o.Ignore())
                    .ForMember(m => m.Created, o => o.Ignore())
                    .ForMember(m => m.LastModified, o => o.Ignore());
            });

            config.AssertConfigurationIsValid();
            Mapper = config.CreateMapper();
        }
    }
}
