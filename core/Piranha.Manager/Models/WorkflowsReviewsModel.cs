using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Piranha.Manager.Models
{
    [Authorize(Policy = Permission.ContentReview)]
    public class WorkflowsReviewViewModel : PageModel
    {
        public void OnGet() { }
    }
}
