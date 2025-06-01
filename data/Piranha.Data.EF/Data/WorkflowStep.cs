namespace Piranha.Data
{
    public class WorkflowStep
    {
        public Guid Id { get; set; } 

        public int? Step { get; set; }

        public string Name { get; set; }
        public string Permission { get; set; }
        public string Reason { get; set; }
    }
}
