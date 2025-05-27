using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Piranha.Manager.Models
{
    [Authorize(Policy = Permission.Content)]
    public class WorkflowsOthersViewModel : PageModel
    {
        public void OnGet() { }
    }
}
