using Microsoft.AspNetCore.Mvc.RazorPages;
using Piranha;
using Piranha.Manager.Models;

namespace Piranha.Manager.Pages.Workflows
{
    public class PublishedPagesModel : PageModel
    {
        private readonly IApi _api;

        public List<PageListModel.PageItem> Pages { get; set; } = new();

        public PublishedPagesModel(IApi api)
        {
            _api = api;
        }

        public async Task OnGetAsync()
        {
            var siteId = (await _api.Sites.GetDefaultAsync()).Id;
            var allPages = await _api.Pages.GetAllAsync(siteId);

            Pages = allPages
                .Where(p => p.Published.HasValue) // <-- só páginas publicadas
                .Select(p => new PageListModel.PageItem
                {
                    Id       = p.Id,
                    Title    = p.Title,
                    TypeName = p.TypeId,
                    Status   = "Published",
                    EditUrl  = $"/manager/page/edit/{p.Id}"
                })
                .ToList();
        }
    }
}
