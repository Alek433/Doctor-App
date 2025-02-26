using System.ComponentModel.DataAnnotations;

namespace Doctor_App.Infrastructure.Data.Entities
{
    public class Billing
    {
        [Key]
        public int Id { get; set; }
        public int VisitId { get; set; }
        public decimal Amount { get; set; }
        public string InsuranceDetails { get; set; }
        public string PaymentStatus { get; set; } // e.g., Paid, Pending, Denied

        // Navigation Property
        public Visit Visit { get; set; }
    }
}
