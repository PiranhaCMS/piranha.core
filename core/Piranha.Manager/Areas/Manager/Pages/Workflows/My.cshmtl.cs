using Microsoft.AspNetCore.Mvc.RazorPages;
using Piranha.Manager.Models;

namespace Piranha.Manager.Pages.Workflows
{
    public class MyContentModel : PageModel
    {
        public List<PageListModel.PageItem> Pages { get; set; } = new();

        public void OnGet()
        {
            Pages = new List<PageListModel.PageItem>
            {
                new PageListModel.PageItem
                {
                    Id = Guid.Parse("b6fcf235-5b0d-4c3f-9185-f88f55eac70f"),
                    Title = "Exemplo de Post",
                    TypeName = "BlogPost",
                    Status = "Pending"
                },
                new PageListModel.PageItem
                {
                    Id = Guid.Parse("32702fe3-60d4-44ff-899e-d3b0c0901af5"),
                    Title = "Post Aprovado",
                    TypeName = "Article",
                    Status = "Approved"
                },
                new PageListModel.PageItem
                {
                    Id = Guid.Parse("93f29234-bfe4-4630-b4d6-29e0cf4e81a1"),
                    Title = "Post Rejeitado",
                    TypeName = "NewsItem",
                    Status = "Rejected"
                }
            };
        }
    }
}
