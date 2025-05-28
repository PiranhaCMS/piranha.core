using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;

namespace Piranha.Manager.Pages.Workflows
{
    public class ReviewModel : PageModel
    {
        public List<PostItem> Posts { get; set; } = new();

        public class PostItem
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string TypeName { get; set; }
            public string Author { get; set; }
            public DateTime SubmittedAt { get; set; }
            public string Status { get; set; } // "Pending", "Approved", "Rejected"
        }

        public void OnGet()
        {
            // Simulação: substitui com dados reais se necessário
            Posts = new List<PostItem>
            {
                new PostItem {
                    Id = "1",
                    Title = "Lorem Ipsum Dolor",
                    TypeName = "BlogPost",
                    Author = "alice@example.com",
                    SubmittedAt = DateTime.Parse("2025-05-27 10:15"),
                    Status = "Pending"
                },
                new PostItem {
                    Id = "2",
                    Title = "Company Update",
                    TypeName = "NewsItem",
                    Author = "bob@example.com",
                    SubmittedAt = DateTime.Parse("2025-05-26 09:00"),
                    Status = "Approved"
                },
                new PostItem {
                    Id = "3",
                    Title = "Internal Memo",
                    TypeName = "Article",
                    Author = "carla@example.com",
                    SubmittedAt = DateTime.Parse("2025-05-25 15:40"),
                    Status = "Rejected"
                }
            };
        }
    }
}
