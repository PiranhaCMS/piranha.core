/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System;
using AutoMapper;

namespace Piranha.Services
{
    public class ContentServiceFactory : IContentServiceFactory
    {
        private readonly IServiceProvider _services;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="services">The service provider</param>
        public ContentServiceFactory(IServiceProvider services)
        {
            _services = services;
        }

        /// <summary>
        /// Creates a new content service for the specified types.
        /// </summary>
        /// <param name="mapper">The AutoMapper instance to use for transformation</param>
        /// <returns>The content service</returns>
        public IContentService<TContent, TField, TModelBase> Create<TContent, TField, TModelBase>(IMapper mapper)
            where TContent : Data.Content<TField>
            where TField : Data.ContentField
            where TModelBase : Models.Content
        {
            return new ContentService<TContent, TField, TModelBase>(_services, mapper);
        }

        /// <summary>
        /// Creates a new page content service.
        /// </summary>
        /// <returns>The content service</returns>
        public IContentService<Data.Page, Data.PageField, Models.PageBase> CreatePageService()
        {
            return new ContentService<Data.Page, Data.PageField, Models.PageBase>(_services, App.Mapper);
        }

        /// <summary>
        /// Creates a new post content service.
        /// </summary>
        /// <returns>The content service</returns>
        public IContentService<Data.Post, Data.PostField, Models.PostBase> CreatePostService()
        {
            return new ContentService<Data.Post, Data.PostField, Models.PostBase>(_services, App.Mapper);
        }

        /// <summary>
        /// Creates a new site content service.
        /// </summary>
        /// <returns>The content service</returns>
        public IContentService<Data.Site, Data.SiteField, Models.SiteContentBase> CreateSiteService()
        {
            return new ContentService<Data.Site, Data.SiteField, Models.SiteContentBase>(_services, App.Mapper);
        }
    }
}
