namespace Piranha.Data
{
    public class Workflow
    {
        public Guid Id { get; set; }
        public int CurrentStep { get; set; } = 0;
        public bool IsApproved { get; set; } = false;

        // Navigation property for related steps
        public List<WorkflowStep> Steps { get; set; } = new List<WorkflowStep>();
    }
}
