using System;
using System.Threading.Tasks;
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

        public async Task OnGet(Guid id)
        {
            Data = await _api.Pages.GetByIdAsync<StandardPage>(id);
        }
    }
}