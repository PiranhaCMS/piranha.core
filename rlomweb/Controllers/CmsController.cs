using rlomweb.Models;
using Microsoft.AspNetCore.Mvc;
using Piranha;
using Piranha.AspNetCore.Services;
using System;
using System.Threading.Tasks;

namespace rlomweb.Controllers
{
    public class CmsController : Controller
    {
        private readonly IApi _api;
        private readonly IModelLoader _loader;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public CmsController(IApi api, IModelLoader loader)
        {
            _api = api;
            _loader = loader;
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
        /// <param name="draft">If a draft is requested</param>
        [Route("archive")]
        public async Task<IActionResult> Archive(Guid id, int? year = null, int? month = null, int? page = null,
            Guid? category = null, Guid? tag = null, bool draft = false)
        {
            var model = await _loader.GetPage<BlogArchive>(id, HttpContext.User, draft);
            model.Archive = await _api.Archives.GetByIdAsync(id, page, category, tag, year, month);

            return View(model);
        }

        /// <summary>
        /// Gets the page with the given id.
        /// </summary>
        /// <param name="id">The unique page id</param>
        /// <param name="draft">If a draft is requested</param>
        [Route("page")]
        public async Task<IActionResult> Page(Guid id, bool draft = false)
        {
            var model = await _loader.GetPage<StandardPage>(id, HttpContext.User, draft);

            return View(model);
        }

        /// <summary>
        /// Gets the post with the given id.
        /// </summary>
        /// <param name="id">The unique post id</param>
        /// <param name="draft">If a draft is requested</param>
        [Route("post")]
        public async Task<IActionResult> Post(Guid id, bool draft = false)
        {
            var model = await _loader.GetPost<BlogPost>(id, HttpContext.User, draft);

            return View(model);
        }

        /// <summary>
        /// Gets the startpage with the given id.
        /// </summary>
        /// <param name="id">The unique page id</param>
        /// <param name="draft">If a draft is requested</param>
        [Route("start")]
        public async Task<IActionResult> Start(Guid id, bool draft = false)
        {
            var model = await _loader.GetPage<StartPage>(id, HttpContext.User, draft);

            return View(model);
        }

        /// <summary>
        /// Gets the homepage.
        /// </summary>
        /// <param name="draft">If a draft is requested</param>
        [Route("home")]
        public async Task<IActionResult> Home(Guid id, bool draft = false)
        {
            var model = await _loader.GetPage<RlomHomePage>(id, HttpContext.User, draft);

            return View(model);
        }
    }
}
