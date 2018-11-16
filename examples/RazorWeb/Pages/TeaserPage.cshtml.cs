using System;
using System.Linq;
using Piranha;
using Piranha.Models;
using RazorWeb.Models;

namespace RazorWeb.Pages
{
    public class TeaserPageModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        private readonly IApi _api;
        private readonly IDb _db;
        public TeaserPage Data { get; private set; }

        public TeaserPageModel(IApi api, IDb db) : base()
        {
            _api = api;
            _db = db;
        }

        public void OnGet(Guid id, bool startpage = false)
        {
            Data = _api.Pages.GetById<TeaserPage>(id);

            if (startpage)
            {
                var latest = _db.Posts
                    .Where(p => p.Published <= DateTime.Now)
                    .OrderByDescending(p => p.Published)
                    .Take(1)
                    .Select(p => p.Id);

                if (latest.Count() > 0)
                {
                    Data.LatestPost = _api.Posts
                        .GetById<PostInfo>(latest.First());
                }
            }            
        }
    }
}