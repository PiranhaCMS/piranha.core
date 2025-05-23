namespace Piranha.Models
{
    public class WorkflowStep{
        public Guid WorkflowId { get; set; }
        public Workflow Workflow { get; set; }

        public string name { get; set; }
        public string permission { get; set; }
        public string? reason { get; set; }
    }
}