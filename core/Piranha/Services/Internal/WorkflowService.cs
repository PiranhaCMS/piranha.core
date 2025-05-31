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
        // Sempre cria um novo workflow
        page.Workflow = CreateWorkflow();
        page.Workflow.CurrentStep = 0;
        page.Workflow.IsApproved = false;

        page.Published = null;
        page.WorkflowStatusValue = (int)PageBase.PageWorkflowStatus.PendingReview;

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
                new WorkflowStep { Permission = "PiranhaReviewer", Name = "Initial Review" },
                new WorkflowStep { Permission = "PiranhaLegalTeam", Name = "Legal Team Review" },
            },
        };
    }

    public async Task Approve(PageBase page, ClaimsPrincipal user, string reason = null)
    {

        Console.WriteLine("[DEBUG] Workflow Steps:");
        for (int i = 0; i < page.Workflow.Steps.Count; i++)
        {
            var step = page.Workflow.Steps[i];
            Console.WriteLine($" - Step {i}: {step.Name}, Permission = {step.Permission}, Reason = {step.Reason}");
        }
        if (page.Workflow == null || page.Workflow.Steps == null || !page.Workflow.Steps.Any())
        {
            throw new InvalidOperationException("Workflow is not properly initialized.");
        }

        var workflow = page.Workflow;

        // Evita erro de índice inválido
        if (workflow.CurrentStep < 0 || workflow.CurrentStep >= workflow.Steps.Count)
        {
            throw new InvalidOperationException($"Invalid workflow CurrentStep: {workflow.CurrentStep}");
        }

        var permission = workflow.GetCurrentPermission();

        Console.WriteLine($"[DEBUG] Page ID: {page.Id}");
        Console.WriteLine($"[DEBUG] Workflow null? {workflow == null}");
        Console.WriteLine($"[DEBUG] Steps count: {workflow.Steps.Count}");
        Console.WriteLine($"[DEBUG] CurrentStep: {workflow.CurrentStep}");
        Console.WriteLine($"[DEBUG] Required Permission: {permission}");

        var userRoles = user.Claims
            .Where(c => c.Type == ClaimTypes.Role || c.Type.EndsWith("/claims/role"))
            .Select(c => c.Value)
            .ToList();

        foreach (var claim in user.Claims)
        {
            Console.WriteLine($"[CLAIM] {claim.Type} = {claim.Value}");
        }

        // Extra debug info
        Console.WriteLine($"[DEBUG] User roles: {string.Join(", ", userRoles)}");

        if (user.Claims.Any(c => c.Value == permission))
        {
            // Guarda a razão no passo atual
            workflow.Approve(reason);

            // Atualiza status
            if (workflow.IsApproved)
            {
                page.Published = DateTime.UtcNow;
                page.WorkflowStatusValue = (int)PageBase.PageWorkflowStatus.Approved;
            }
            else
            {
                // Define próximo status com base no novo step
                switch (workflow.CurrentStep)
                {
                    case 0:
                        page.WorkflowStatusValue = (int)PageBase.PageWorkflowStatus.PendingReview;
                        break;
                    case 1:
                        page.WorkflowStatusValue = (int)PageBase.PageWorkflowStatus.PendingLegal;
                        break;
                    default:
                        page.WorkflowStatusValue = (int)PageBase.PageWorkflowStatus.Draft;
                        break;
                }
            }

            await _api.Pages.SaveAsync(page);
        }
        else
        {
            Console.WriteLine($"[ERROR] User does not have required permission '{permission}'");
            throw new UnauthorizedAccessException("User does not have the required permission to approve this content.");
        }
    }


    public async Task Deny(PageBase page, ClaimsPrincipal user, string reason = null)
    {
        Workflow workflow = page.Workflow;

        string permission = workflow.GetCurrentPermission();

        if (user.Claims.Any(c => c.Value == permission))
        {
            workflow.Deny(reason);

            // Atualizar status na base de dados
            page.WorkflowStatusValue = (int)PageBase.PageWorkflowStatus.Rejected;

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
