using System;
using System.Text.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContentRus.TenantManagement.Models
{
    public class TenantPlan
    // used for describe prices and other details of a tenant plan
    {
        public required TenantTier Id { get; set; }
        public required string Name { get; set; }
        public required double Price { get; set; }
        public required string PriceId { get; set; }
        public string FeaturesJson { get; set; }

        [NotMapped]
        public List<string> Features
        {
            get => JsonSerializer.Deserialize<List<string>>(FeaturesJson ?? "[]")!;
            set => FeaturesJson = JsonSerializer.Serialize(value);
        }
    }
}
