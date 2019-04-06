using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task OnGet(Guid id, bool startpage = false)
        {
            Data = await _api.Pages.GetByIdAsync<TeaserPage>(id);

            if (startpage)
            {
                var latest = await _db.Posts
                    .Where(p => p.Published <= DateTime.Now)
                    .OrderByDescending(p => p.Published)
                    .Take(1)
                    .Select(p => p.Id)
                    .ToListAsync();

                if (latest.Count() > 0)
                {
                    Data.LatestPost = await _api.Posts
                        .GetByIdAsync<PostInfo>(latest.First());
                }
            }
        }
    }
}