using Microsoft.AspNetCore.Mvc;
using Piranha;
using Piranha.Models;
using System;
using System.Linq;

namespace MvcWeb.Controllers
{
    public class CmsController : Controller
    {
        private readonly IApi _api;
        private readonly IDb _db;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="app">The current app</param>
        public CmsController(IApi api, IDb db) 
        {            
            _api = api;
            _db = db;
        }

        /// <summary>
        /// Gets the blog archive with the given id.
        /// </summary>
        /// <param name="id">The unique page id</param>
        /// <param name="year">The optional year</param>
        /// <param name="month">The optional month</param>
        /// <param name="page">The optional page</param>
        /// <param name="category">The optional category</param>
        /// <param name="tag">The optional tag</param>
        [Route("archive")]
        public IActionResult Archive(Guid id, int? year = null, int? month = null, int? page = null, 
            Guid? category = null, Guid? tag = null) 
        {
            Models.BlogArchive model;

            if (category.HasValue)
                model = _api.Archives.GetByCategoryId<Models.BlogArchive>(id, category.Value, page, year, month);
            else if (tag.HasValue)
                model = _api.Archives.GetByTagId<Models.BlogArchive>(id, tag.Value, page, year, month);
            else model = _api.Archives.GetById<Models.BlogArchive>(id, page, year, month);
            
            return View(model);
        }

        /// <summary>
        /// Gets the page with the given id.
        /// </summary>
        /// <param name="id">The unique page id</param>
        [Route("page")]
        public IActionResult Page(Guid id) 
        {
            var model = _api.Pages.GetById<Models.StandardPage>(id);

            return View(model);
        }

        /// <summary>
        /// Gets the page with the given id.
        /// </summary>
        /// <param name="id">The unique page id</param>
        [Route("pagewide")]
        public IActionResult PageWide(Guid id) 
        {
            var model = _api.Pages.GetById<Models.StandardPage>(id);

            return View(model);
        }

        /// <summary>
        /// Gets the post with the given id.
        /// </summary>
        /// <param name="id">The unique post id</param>
        /// 
        [Route("post")]
        public IActionResult Post(Guid id) 
        {
            var model = _api.Posts.GetById<Models.BlogPost>(id);

            return View(model);
        }

        /// <summary>
        /// Gets the teaser page with the given id.
        /// </summary>
        /// <param name="id">The page id</param>
        /// <param name="startpage">If this is the startpage of the site</param>
        [Route("teaserpage")]
        public IActionResult TeaserPage(Guid id, bool startpage = false)
        {
            var model = _api.Pages.GetById<Models.TeaserPage>(id);

            if (startpage)
            {
                var latest = _db.Posts
                    .Where(p => p.Published <= DateTime.Now)
                    .OrderByDescending(p => p.Published)
                    .Take(1)
                    .Select(p => p.Id);
                if (latest.Count() > 0)
                {
                    model.LatestPost = _api.Posts
                        .GetById<PostInfo>(latest.First());
                }
                return View("startpage", model);
            }
            return View(model);
        }
    }
}
