using Microsoft.AspNetCore.Mvc.RazorPages;
using Piranha.Manager.Models;

namespace Piranha.Manager.Pages.Workflows
{
    public class ReviewModel : PageModel
    {
        public List<PageListModel.PageItem> Pages { get; set; } = new();

        public void OnGet()
        {
            Pages = new List<PageListModel.PageItem>
            {
                new PageListModel.PageItem {
                    Id = Guid.Parse("5b1879e5-bd2d-47d5-a9e6-87c53ce6a8d3"),
                    Title = "Lorem Ipsum Dolor",
                    TypeName = "BlogPost",
                    Status = "Pending",
                    EditUrl = "/manager/page/edit/1"
                },
                new PageListModel.PageItem {
                    Id = Guid.Parse("64835a59-230c-46bc-b7c0-e2643d437eed"),
                    Title = "Company Update",
                    TypeName = "NewsItem",
                    Status = "Approved",
                    EditUrl = "/manager/page/edit/2"
                },
                new PageListModel.PageItem {
                    Id = Guid.Parse("b8984ac1-0c57-41b1-a68e-d0871a1878a5"),
                    Title = "Internal Memo",
                    TypeName = "Article",
                    Status = "Rejected",
                    EditUrl = "/manager/page/edit/3"
                }
            };
        }
    }
}
