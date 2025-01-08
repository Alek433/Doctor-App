using System.ComponentModel.DataAnnotations;

namespace Doctor_App.Data.Models
{
    public class Billing
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int VisitId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string InsuranceDetails { get; set; }
        [Required]
        public string PaymentStatus { get; set; } // e.g., Paid, Pending, Denied

        // Navigation Property
        public Visit Visit { get; set; }
    }
}
