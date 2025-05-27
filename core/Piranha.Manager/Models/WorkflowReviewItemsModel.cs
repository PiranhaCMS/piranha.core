// Areas/Manager/Models/WorkflowsReviewViewModel.cs
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Piranha.Manager.Models
{
    [Authorize(Policy = Permission.ContentReview)]
    public class WorkflowsReviewViewModel : PageModel
    {
        private readonly IWorkflowService _workflowService;

        // Esta propriedade será usada na sua Razor view
        public IEnumerable<WorkflowReviewItem> ReviewItems { get; private set; }

        // Injete aqui o seu serviço de workflow
        public WorkflowsReviewViewModel(IWorkflowService workflowService)
        {
            _workflowService = workflowService;
        }

        public void OnGet()
        {
            // Chama o método que retorna os itens pendentes
            ReviewItems = _workflowService.GetPendingReviews();
            // (ou GetPendingReviewsAsync() + await, se for async)
        }
    }
}
