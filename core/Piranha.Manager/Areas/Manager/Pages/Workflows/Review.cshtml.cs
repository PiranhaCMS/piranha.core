using Microsoft.AspNetCore.Mvc.RazorPages;
using Piranha;
using PiranhaPageBase = Piranha.Models.PageBase;

namespace Piranha.Manager.Pages.Workflows
{
    public class ReviewModel : PageModel
    {
        private readonly IApi _api;

        public ReviewModel(IApi api)
        {
            _api = api;
        }

        public List<PageReviewItem> Pages { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Buscar todas as páginas
            var allPages = await _api.Pages.GetAllAsync();

            Pages = allPages
                .Select(p => new PageReviewItem
                {
                    Id = p.Id,
                    Title = p.Title,
                    TypeName = p.TypeId,
                    Status = GetWorkflowStatus(p),
                    EditUrl = $"/manager/page/edit/{p.Id}",
                    SubmissionDate = p.LastModified,
                    Author = GetPageAuthor(p),
                    HasWorkflow = p.Workflow != null,
                    RawWorkflowStatus = GetRawWorkflowStatus(p),
                    // Adicionar informações de debug
                    WorkflowCurrentStep = p.Workflow?.CurrentStep,
                    WorkflowIsApproved = p.Workflow?.IsApproved ?? false,
                    WorkflowSteps = GetWorkflowStepsInfo(p.Workflow),
                })
                .OrderByDescending(p => p.SubmissionDate)
                .ToList();
        }

        private string GetWorkflowStatus(PiranhaPageBase page)
        {
            // Verificar se tem workflow
            if (page.Workflow == null)
                return "Draft";

            // Usar a propriedade WorkflowStatus do PageBase que já tem a lógica implementada
            return page.WorkflowStatus switch
            {
                PiranhaPageBase.PageWorkflowStatus.PendingReview => "PendingReview",
                PiranhaPageBase.PageWorkflowStatus.PendingLegal => "PendingLegal",
                PiranhaPageBase.PageWorkflowStatus.Approved => "Approved",
                PiranhaPageBase.PageWorkflowStatus.Rejected => "Rejected",
                _ => "Draft",
            };
        }

        private string GetRawWorkflowStatus(PiranhaPageBase page)
        {
            if (page.Workflow == null)
                return "No Workflow";

            var workflow = page.Workflow;
            var currentStepInfo = workflow.GetCurrentStep();

            return $"CurrentStep: {workflow.CurrentStep}, "
                + $"IsApproved: {workflow.IsApproved}, "
                + $"StepReason: {currentStepInfo?.Reason ?? "None"}, "
                + $"WorkflowStatus: {page.WorkflowStatus}";
        }

        private string GetPageAuthor(PiranhaPageBase page)
        {
            // Usa informação do Workflow se quiseres adaptar
            if (page.Workflow?.Steps?.Any() == true)
            {
                var firstStep = page.Workflow.Steps.FirstOrDefault();
                return firstStep?.Permission ?? "System";
            }

            return "System";
        }

        private string GetWorkflowStepsInfo(Piranha.Models.Workflow workflow)
        {
            if (workflow == null || workflow.Steps == null || !workflow.Steps.Any())
                return "No steps";

            var stepsInfo = workflow
                .Steps.Select(
                    (s, i) =>
                        $"Step {i + 1}: {s.Name} - {s.Permission} - Reason: {s.Reason ?? "None"}"
                )
                .ToList();

            return string.Join(" | ", stepsInfo);
        }
    }

    // Classe auxiliar para os dados do review - expandida com mais informações
    public class PageReviewItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string TypeName { get; set; }
        public string Status { get; set; }
        public string EditUrl { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string Author { get; set; }
        public bool HasWorkflow { get; set; }
        public string RawWorkflowStatus { get; set; }

        // Propriedades adicionais para debug
        public int? WorkflowCurrentStep { get; set; }
        public bool WorkflowIsApproved { get; set; }
        public string WorkflowSteps { get; set; }
    }
}
