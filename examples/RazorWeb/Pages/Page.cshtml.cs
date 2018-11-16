using System;
using Piranha;
using RazorWeb.Models;

namespace RazorWeb.Pages
{
    public class PageModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        private readonly IApi _api;
        public StandardPage Data { get; private set; }

        public PageModel(IApi api) : base()
        {
            _api = api;
        }

        public void OnGet(Guid id)
        {
            Data = _api.Pages.GetById<StandardPage>(id);
        }
    }
}