using System.Security.Claims;
using Piranha.Models;
namespace Piranha.Services;

class WorkflowService : IWorkflowService
{
    private readonly IApi _api;
    
    public WorkflowService(IApi api)
    {
        _api = api;
        App.Hooks.Pages.RegisterOnBeforeSave(page =>
        {
            page.Published = null; //unpublish

            if (page.Workflow == null)
            {
                page.Workflow = new Workflow
                {
                    Steps = new List<WorkflowStep>() //since this is an MVP, I'll hard-code the steps (to start simply, we'll just have an initial reviewer and then the legal team)
                    {
                        new WorkflowStep {
                            Permission ="Workflow.Reviewer",
                            Name="Initial Review"
                        },
                        new WorkflowStep {
                            Permission ="Workflow.LegalTeam",
                            Name="Legal Team Review"
                        }
                    }
                };
            }
            else
            { //any changes requires going back to the first step
                page.Workflow.CurrentStep = 0;
                page.Workflow.IsApproved = false;
            }


        });
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
            throw new UnauthorizedAccessException("User does not have the required permission to approve this content");
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
            throw new UnauthorizedAccessException("User does not have the required permission to deny this content");
        }
    }
}