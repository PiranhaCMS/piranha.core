/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using AutoMapper;

namespace Piranha.Services
{
    public interface IContentServiceFactory
    {
        /// <summary>
        /// Creates a new content service for the specified types.
        /// </summary>
        /// <param name="mapper">The AutoMapper instance to use for transformation</param>
        /// <returns>The content service</returns>
        IContentService<TContent, TField, TModelBase> Create<TContent, TField, TModelBase>(IMapper mapper)
            where TContent : Data.ContentBase<TField>
            where TField : Data.ContentFieldBase
            where TModelBase : Models.ContentBase;

        /// <summary>
        /// Creates a new page content service.
        /// </summary>
        /// <returns>The content service</returns>
        IContentService<Data.Page, Data.PageField, Models.PageBase> CreatePageService();

        /// <summary>
        /// Creates a new post content service.
        /// </summary>
        /// <returns>The content service</returns>
        IContentService<Data.Post, Data.PostField, Models.PostBase> CreatePostService();

        /// <summary>
        /// Creates a new site content service.
        /// </summary>
        /// <returns>The content service</returns>
        IContentService<Data.Site, Data.SiteField, Models.SiteContentBase> CreateSiteService();
    }
}
