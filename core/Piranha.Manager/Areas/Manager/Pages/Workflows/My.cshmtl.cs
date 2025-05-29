using Microsoft.AspNetCore.Mvc.RazorPages;
using Piranha.Manager.Models;

namespace Piranha.Manager.Pages.Workflows
{
    public class MyContentModel : PageModel
    {
        public List<PostListModel.PostItem> Posts { get; set; } = new();

        public void OnGet()
        {
            Posts = new List<PostListModel.PostItem>
            {
                new PostListModel.PostItem
                {
                    Id = "1",
                    Title = "Exemplo de Post",
                    TypeName = "BlogPost",
                    Status = "Pending"
                },
                new PostListModel.PostItem
                {
                    Id = "2",
                    Title = "Post Aprovado",
                    TypeName = "Article",
                    Status = "Approved"
                },
                new PostListModel.PostItem
                {
                    Id = "3",
                    Title = "Post Rejeitado",
                    TypeName = "NewsItem",
                    Status = "Rejected"
                }
            };
        }
    }
}
