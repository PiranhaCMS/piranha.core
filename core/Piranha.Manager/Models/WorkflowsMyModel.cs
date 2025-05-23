using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Piranha.Manager.Models
{
    [Authorize(Policy = Permission.ContentApproval)]
    [Authorize(Policy = Permission.ContentApprovalAdmin)] 
    public class WorkflowsMyViewModel : PageModel
    {
        public void OnGet() { }
    }
}

