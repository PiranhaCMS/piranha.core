using System.Security.Claims;
using Piranha.Models;

namespace Piranha.Services;

// MUDANÇA: de 'class' para 'public class'
public class WorkflowService : IWorkflowService
{
    private readonly IApi _api;

    public WorkflowService(IApi api)
    {
        _api = api;

        // Hook simplificado - só despublica ao gravar, não cria workflow
        App.Hooks.Pages.RegisterOnBeforeSave(page =>
        {
            // Se a página tem workflow ativo e foi editada, volta ao primeiro step
            if (page.Workflow != null && page.Workflow.Steps.Any())
            {
                page.Published = null; // unpublish
                page.Workflow.CurrentStep = 0;
                page.Workflow.IsApproved = false;
            }
        });
    }

    /// <summary>
    /// Cria um novo workflow para uma página e submete para revisão
    /// </summary>
    public async Task SubmitForReview(PageBase page)
    {
        // Cria o workflow se não existir
        if (page.Workflow == null)
        {
            page.Workflow = CreateWorkflow();
        }
        else
        {
            // Reset workflow se já existir
            page.Workflow.CurrentStep = 0;
            page.Workflow.IsApproved = false;
        }

        // Despublica a página
        page.Published = null;

        await _api.Pages.SaveAsync(page);
    }

    /// <summary>
    /// Cria um novo workflow com os steps definidos
    /// </summary>
    private Workflow CreateWorkflow()
    {
        return new Workflow
        {
            Steps = new List<WorkflowStep>
            {
                new WorkflowStep { Permission = "Workflow.Reviewer", Name = "Initial Review" },
                new WorkflowStep { Permission = "Workflow.LegalTeam", Name = "Legal Team Review" },
            },
        };
    }

    public async Task Approve(PageBase page, ClaimsPrincipal user, string reason = null)
    {
        Workflow workflow = page.Workflow;

        string permission = workflow.GetCurrentPermission();

        if (user.HasClaim("Permission", permission))
        {
            workflow.Approve(reason);

            if (workflow.IsApproved)
            {
                page.Published = DateTime.UtcNow;
            }
            await _api.Pages.SaveAsync(page);
        }
        else
        {
            throw new UnauthorizedAccessException(
                "User does not have the required permission to approve this content"
            );
        }
    }

    public async Task Deny(PageBase page, ClaimsPrincipal user, string reason = null)
    {
        Workflow workflow = page.Workflow;

        string permission = workflow.GetCurrentPermission();

        if (user.HasClaim("Permission", permission))
        {
            workflow.Deny(reason);
            await _api.Pages.SaveAsync(page);
        }
        else
        {
            throw new UnauthorizedAccessException(
                "User does not have the required permission to deny this content"
            );
        }
    }
}
