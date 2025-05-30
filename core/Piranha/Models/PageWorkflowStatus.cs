namespace Piranha.Models
{
    /// <summary>
    /// Represents the status of a page in the workflow.
    /// </summary>
    public enum PageWorkflowStatus
    {
        Draft,
        PendingReview,
        PendingLegal,
        Approved,
        Rejected,
    }
}
