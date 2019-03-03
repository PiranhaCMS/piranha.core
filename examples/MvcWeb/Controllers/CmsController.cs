using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Piranha;
using Piranha.Models;
using Piranha.Services;

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
        public async Task<IActionResult> Archive(Guid id, int? year = null, int? month = null, int? page = null,
            Guid? category = null, Guid? tag = null)
        {
            return View(await _api.Archives.GetByIdAsync<Models.BlogArchive>(id, page, category, tag, year, month));
        }

        /// <summary>
        /// Gets the page with the given id.
        /// </summary>
        /// <param name="id">The unique page id</param>
        [Route("page")]
        public async Task<IActionResult> Page(Guid id)
        {
            var model = await _api.Pages.GetByIdAsync<Models.StandardPage>(id);

            return View(model);
        }

        /// <summary>
        /// Gets the page with the given id.
        /// </summary>
        /// <param name="id">The unique page id</param>
        [Route("pagewide")]
        public async Task<IActionResult> PageWide(Guid id)
        {
            var model = await _api.Pages.GetByIdAsync<Models.StandardPage>(id);

            return View(model);
        }

        /// <summary>
        /// Gets the post with the given id.
        /// </summary>
        /// <param name="id">The unique post id</param>
        ///
        [Route("post")]
        public async Task<IActionResult> Post(Guid id)
        {
            var model = await _api.Posts.GetByIdAsync<Models.BlogPost>(id);

            return View(model);
        }

        /// <summary>
        /// Gets the teaser page with the given id.
        /// </summary>
        /// <param name="id">The page id</param>
        /// <param name="startpage">If this is the startpage of the site</param>
        [Route("teaserpage")]
        public async Task<IActionResult> TeaserPage(Guid id, bool startpage = false)
        {
            var model = await _api.Pages.GetByIdAsync<Models.TeaserPage>(id);

            if (startpage)
            {
                var latest = _db.Posts
                    .Where(p => p.Published <= DateTime.Now)
                    .OrderByDescending(p => p.Published)
                    .Take(1)
                    .Select(p => p.Id);
                if (latest.Count() > 0)
                {
                    model.LatestPost = await _api.Posts
                        .GetByIdAsync<PostInfo>(latest.First());
                }
                return View("startpage", model);
            }
            return View(model);
        }
    }
}
