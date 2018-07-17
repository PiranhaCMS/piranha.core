/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/tidyui/coreweb
 * 
 */

using CoreWebAngular.Converters;
using CoreWebAngular.Models.Blocks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Piranha;
using Piranha.Extend.Blocks;
using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreWebAngular.Controllers
{
    /// <summary>
    /// Simple controller for handling the CMS API content from Piranha.
    /// </summary>
    [Route("api/[controller]")]
    public class CmsController : Controller
    {
        /// <summary>
        /// The private api.
        /// </summary>
        private readonly IApi api;

        /// <summary>
        /// The private serializerSettings.
        /// </summary>
        private readonly JsonSerializerSettings serializerSettings;

        /// <summary>
        /// Default construtor.
        /// </summary>
        /// <param name="api">The current api</param>
        public CmsController(IApi api)
        {
            this.api = api;

            serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            serializerSettings.Converters.Add(new ClassNameCoverter<HtmlBlock>());
            serializerSettings.Converters.Add(new ClassNameCoverter<HtmlColumnBlock>());
            serializerSettings.Converters.Add(new ClassNameCoverter<ImageBlock>());
            serializerSettings.Converters.Add(new ClassNameCoverter<SizedImageBlock>());
            serializerSettings.Converters.Add(new ClassNameCoverter<QuoteBlock>());
            serializerSettings.Converters.Add(new ClassNameCoverter<TextBlock>());
            serializerSettings.Converters.Add(new SizedImageCoverter(api));
            serializerSettings.Converters.Add(new ImageCoverter(api));
        }

        /// <summary>
        /// Gets the sitemap with the specified id or default sitemap if Empty.
        /// </summary>
        /// <param name="id">The unique id</param>
        [HttpGet("sitemap")]
        public IActionResult Sitemap(Guid? id = null)
        {
            var model = api.Sites.GetSitemap(id);

            foreach (var partial in model)
            {
                if (partial.PageTypeName == "Blog Archive")
                {
                    ((List<SitemapItem>)partial.Items).AddRange(GetArchiveItems(partial.Id, partial.Level));
                }
            }

            var json = JsonConvert.SerializeObject(model, serializerSettings);
            return new OkObjectResult(json);
        }

        private List<SitemapItem> GetArchiveItems(Guid id, int level)
        {
            var model = new List<SitemapItem>();
            var posts = api.Posts.GetAll(id).Where(p => p.Published <= DateTime.Now).ToList();

            for (int i = 0; i < posts.Count; i++)
            {
                DynamicPost item = posts[i];
                var smItem = new SitemapItem
                {
                    ParentId = id,
                    SortOrder = i,
                    Title = item.Title,
                    NavigationTitle = item.Title,
                    PageTypeName = item.TypeId,
                    Permalink = item.Permalink,
                    Published = item.Published,
                    Created = item.Created,
                    LastModified = item.LastModified,
                    Id = item.Id,
                    Level = level + 1
                };
                model.Add(smItem);
            }
            return model;
        }

        /// <summary>
        /// Gets the archive for the category with the specified id.
        /// </summary>
        /// <param name="id">The category id</param>
        /// <param name="year">The optional year</param>
        /// <param name="month">The optional month</param>
        /// <param name="page">The optional page</param>
        /// <param name="category">The optional category id</param>
        [HttpGet("archive")]
        public IActionResult Archive(Guid id, int? year = null, int? month = null, int? page = null, Guid? category = null, Guid? tag = null)
        {
            Models.BlogArchive model;

            if (category.HasValue)
                model = api.Archives.GetByCategoryId<Models.BlogArchive>(id, category.Value, page, year, month);
            else if (tag.HasValue)
                model = api.Archives.GetByTagId<Models.BlogArchive>(id, tag.Value, page, year, month);
            else model = api.Archives.GetById<Models.BlogArchive>(id, page, year, month);

            var json = JsonConvert.SerializeObject(model, serializerSettings);
            return new OkObjectResult(json);
        }


        /// <summary>
        /// Gets the page with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        [HttpGet("page")]
        public IActionResult Page(Guid id)
        {
            var model = api.Pages.GetById<Models.StandardPage>(id);

            var json = JsonConvert.SerializeObject(model, serializerSettings);
            return new OkObjectResult(json);
        }

        /// <summary>
        /// Gets the post with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        [HttpGet("post")]
        public IActionResult Post(Guid id)
        {
            var model = api.Posts.GetById<Models.BlogPost>(id);

            var json = JsonConvert.SerializeObject(model, serializerSettings);
            return new OkObjectResult(json);
        }

        /// <summary>
        /// Gets the TeaserPage with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        [HttpGet("teaserpage")]
        public IActionResult TeaserPage(Guid id)
        {
            var model = api.Pages.GetById<Models.TeaserPage>(id);

            //foreach (var teaser in model.Teasers)
            //{
            //    if (teaser.Image.HasValue)
            //    {
            //        teaser.Image.Size = 256;
            //    }
            //}

            var json = JsonConvert.SerializeObject(model, serializerSettings);
            return new OkObjectResult(json);
        }
    }
}
