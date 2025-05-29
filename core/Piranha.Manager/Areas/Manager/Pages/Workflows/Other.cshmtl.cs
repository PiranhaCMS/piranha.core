using Microsoft.AspNetCore.Mvc.RazorPages;
using Piranha.Manager.Models;

namespace Piranha.Manager.Pages.Workflows
{
    public class PublishedPagesModel : PageModel
    {
        public List<PageListModel.PageItem> Pages { get; set; } = new();

        public void OnGet()
        {
            Pages = new List<PageListModel.PageItem>
            {
                new PageListModel.PageItem {
                    Id = Guid.Parse("b5acfcf0-bf3d-4a11-95a9-7cfba7201e50"),
                    Title = "Lorem Ipsum Dolor",
                    TypeName = "BlogPost",
                    Status = PageListModel.PageItem.Unpublished,
                    EditUrl = "/manager/page/edit/1"
                },
                new PageListModel.PageItem {
                    Id = Guid.Parse("df29f8ae-4ac6-4e66-bb0f-99c72ebcfe49"),
                    Title = "Company Update",
                    TypeName = "NewsItem",
                    Status = PageListModel.PageItem.Draft,
                    EditUrl = "/manager/page/edit/2"
                },
                new PageListModel.PageItem {
                    Id = Guid.Parse("fa3bda46-b125-4d7b-9cc7-4d0e08752b62"),
                    Title = "Tech Trends 2025",
                    TypeName = "Article",
                    Status = "Published",
                    EditUrl = "/manager/page/edit/3"
                }
            };
        }
    }
}
