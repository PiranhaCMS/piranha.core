using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha;
using Piranha.Services;

namespace Piranha.Manager.Controllers
{
    [Area("Manager")]
    [Route("manager/api/workflow")]
    [Authorize(Policy = "PiranhaManager")]
    public class WorkflowController : Controller
    {
        private readonly IApi _api;
        private readonly IWorkflowService _workflowService;

        public WorkflowController(IApi api, IWorkflowService workflowService)
        {
            _api = api;
            _workflowService = workflowService;
        }

        /// <summary>
        /// Submits a page for review
        /// </summary>
        [HttpPost("submit/{id:guid}")]
        public async Task<IActionResult> SubmitForReview(Guid id)
        {
            try
            {
                var page = await _api.Pages.GetByIdAsync(id);
                if (page == null)
                {
                    return NotFound();
                }

                await _workflowService.SubmitForReview(page);

                return Ok(
                    new { success = true, message = "Page submitted for review successfully" }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Approves the current workflow step
        /// </summary>
        [HttpPost("approve/{id:guid}")]
        public async Task<IActionResult> Approve(Guid id, [FromBody] ApprovalRequest request)
        {
            try
            {
                var page = await _api.Pages.GetByIdAsync(id);
                if (page == null)
                {
                    return NotFound();
                }

                await _workflowService.Approve(page, User, request?.Reason);

                return Ok(new { success = true, message = "Page approved successfully" });
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("UnauthorizedAccessException caught in Approve action");
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Denies the current workflow step
        /// </summary>
        [HttpPost("deny/{id:guid}")]
        public async Task<IActionResult> Deny(Guid id, [FromBody] ApprovalRequest request)
        {
            try
            {
                var page = await _api.Pages.GetByIdAsync(id);
                if (page == null)
                {
                    return NotFound();
                }

                await _workflowService.Deny(page, User, request?.Reason);

                return Ok(new { success = true, message = "Page denied successfully" });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

//         [HttpPost("fix")]
// public async Task<IActionResult> FixWorkflows()
// {
//     var pages      = await _api.Pages.GetAllAsync();
//     int fixedCount = 0;

//     foreach (var p in pages)
//     {
//         // Se não tem workflow correcto, recria-o usando o serviço
//         if (p.Workflow == null ||
//             p.Workflow.Steps.Count < 2 ||
//             p.Workflow.Steps[0].Permission != "PiranhaReviewer")
//         {
//             await _workflowService.SubmitForReview(p);  // ← isto chama CreateWorkflow lá dentro
//             fixedCount++;
//         }
//     }

//     return Ok(new { fixedCount });
// }
    }

    public class ApprovalRequest
    {
        public string Reason { get; set; }
    }
}
