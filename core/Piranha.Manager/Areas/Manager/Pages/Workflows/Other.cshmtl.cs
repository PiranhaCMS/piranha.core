using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;

namespace Piranha.Manager.Pages.Workflows
{
    public class PublishedPostModel : PageModel
    {
        public List<PostItem> Posts { get; set; } = new();

        public class PostItem
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string TypeName { get; set; }
            public string Author { get; set; }
            public DateTime PublishedAt { get; set; }
        }

        public void OnGet()
        {
            // Simulação de dados publicados
            Posts = new List<PostItem>
            {
                new PostItem {
                    Id = "1",
                    Title = "Lorem Ipsum Dolor",
                    TypeName = "BlogPost",
                    Author = "alice@example.com",
                    PublishedAt = DateTime.Parse("2025-05-27 13:20")
                },
                new PostItem {
                    Id = "2",
                    Title = "Company Update",
                    TypeName = "NewsItem",
                    Author = "bob@example.com",
                    PublishedAt = DateTime.Parse("2025-05-26 08:45")
                },
                new PostItem {
                    Id = "3",
                    Title = "Tech Trends 2025",
                    TypeName = "Article",
                    Author = "carla@example.com",
                    PublishedAt = DateTime.Parse("2025-05-24 18:10")
                }
            };
        }
    }
}
