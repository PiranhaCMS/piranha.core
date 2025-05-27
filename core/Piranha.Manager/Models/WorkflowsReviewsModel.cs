using System;

namespace Piranha.Manager.Models
{
    public class WorkflowReviewItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ContentType { get; set; }
        public string AuthorName { get; set; }
        public DateTime Submitted { get; set; }
    }
}