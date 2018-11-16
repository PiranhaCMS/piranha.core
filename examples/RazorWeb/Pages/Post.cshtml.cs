using System;
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

        public void OnGet(Guid id)
        {
            Data = _api.Posts.GetById<BlogPost>(id);
        }
    }
}