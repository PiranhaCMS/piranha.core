using Piranha.Models;

using System.Security.Claims;

namespace Piranha.Services;

/// <summary>
/// Interface for the workflow service.
/// </summary>
public interface IWorkflowService
{
    public Task Approve(PostBase post, ClaimsPrincipal user, string reason = null);
    public Task Deny(PostBase post, ClaimsPrincipal user, string reason = null);


}