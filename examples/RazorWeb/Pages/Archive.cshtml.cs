using System;
using System.Threading.Tasks;
using Piranha;
using RazorWeb.Models;

namespace RazorWeb.Pages
{
    public class ArchiveModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        private readonly IApi _api;
        public BlogArchive Data { get; private set; }

        public ArchiveModel(IApi api) : base()
        {
            _api = api;
        }

        public async Task OnGet(Guid id, int? year = null, int? month = null, int? page = null,
            Guid? category = null, Guid? tag = null)
        {
            Data = await _api.Pages.GetByIdAsync<Models.BlogArchive>(id);
            Data.Archive = await _api.Archives.GetByIdAsync(id, page, category, tag, year, month);
        }
    }
}