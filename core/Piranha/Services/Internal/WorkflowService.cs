using System.Security.Claims;
using Piranha.Models;
namespace Piranha.Services;

class WorkflowService : IWorkflowService
{
    private readonly IApi _api;
    
    public WorkflowService(IApi api)
    {
        _api = api;
        App.Hooks.Posts.RegisterOnBeforeSave(post =>
        {
            post.Published = null; //unpublish

            if (post.Workflow == null)
            {
                post.Workflow = new Workflow
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
                post.Workflow.CurrentStep = 0;
                post.Workflow.IsApproved = false;
            }


        });
    }

    public async Task Approve(PostBase post, ClaimsPrincipal user, string reason = null)
    {
        Workflow workflow = post.Workflow;

        string permission = workflow.GetCurrentPermission();

        if (user.HasClaim("Permission", permission))
        {
            workflow.Approve(reason);

            if (workflow.IsApproved)
            {
                post.Published = DateTime.UtcNow;
            }
            await _api.Posts.SaveAsync(post);
        }
        else
        {
            throw new UnauthorizedAccessException("User does not have the required permission to approve this content");
        }
    }

    public async Task Deny(PostBase post, ClaimsPrincipal user, string reason = null)
    {
        Workflow workflow = post.Workflow;

        string permission = workflow.GetCurrentPermission();

        if (user.HasClaim("Permission", permission))
        {
            workflow.Deny();
            await _api.Posts.SaveAsync(post);
        }
        else
        {
            throw new UnauthorizedAccessException("User does not have the required permission to deny this content");
        }
    }
}