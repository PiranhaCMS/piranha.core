using Piranha.Models;
using System.Security.Claims;

namespace Piranha.Services;

/// <summary>
/// Interface for the workflow service.
/// </summary>
public interface IWorkflowService
{
    /// <summary>
    /// Submits a page for review workflow
    /// </summary>
    public Task SubmitForReview(PageBase page);
    
    /// <summary>
    /// Approves the current step of a page workflow
    /// </summary>
    public Task Approve(PageBase page, ClaimsPrincipal user, string reason = null);
    
    /// <summary>
    /// Denies the current step of a page workflow
    /// </summary>
    public Task Deny(PageBase page, ClaimsPrincipal user, string reason = null);
}