using System;
using System.Threading.Tasks;
using Piranha;
using RazorWeb.Models;

namespace RazorWeb.Pages
{
    public class PostModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        private readonly IApi _api;
        public BlogPost Data { get; private set; }

        public PostModel(IApi api) : base()
        {
            _api = api;
        }

        public async Task OnGet(Guid id)
        {
            Data = await _api.Posts.GetByIdAsync<BlogPost>(id);
        }
    }
}