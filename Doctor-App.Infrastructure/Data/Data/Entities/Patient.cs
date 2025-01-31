using System.ComponentModel.DataAnnotations;

namespace Doctor_App.Data.Models
{
    public class Patient
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } // Consider using an enum
        public string ContactInformation { get; set; }
        public string Address { get; set; }
        public string EmergencyContact { get; set; }

        // Navigation Property
        public ICollection<Visit> Visits { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }
}
