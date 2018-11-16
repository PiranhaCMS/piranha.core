using System;
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

        public void OnGet(Guid id, int? year = null, int? month = null, int? page = null, 
            Guid? category = null, Guid? tag = null)
        {
            if (category.HasValue)
                Data = _api.Archives.GetByCategoryId<BlogArchive>(id, category.Value, page, year, month);
            else if (tag.HasValue)
                Data = _api.Archives.GetByTagId<BlogArchive>(id, tag.Value, page, year, month);
            else Data = _api.Archives.GetById<BlogArchive>(id, page, year, month);
        }
    }
}