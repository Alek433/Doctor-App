using System.ComponentModel.DataAnnotations;

namespace Doctor_App.Data.Models
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        [StringLength(50)]
        public string Gender { get; set; } // Consider using an enum
        [Required,StringLength(50)]
        public string ContactInformation { get; set; }
        [Required]
        [StringLength(50)]
        public string Address { get; set; }
        [Required]
        [StringLength(50)]
        public string EmergencyContact { get; set; }

        // Navigation Property
        public ICollection<Visit> Visits { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }
}
