using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doctor_App.Infrastructure.Data.Entities
{
    public class Patient
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string UserId { get; set; } // Foreign key to IdentityUser
        // Navigation property to AspNetUsers table
        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }
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
        public ICollection<PatientDoctor> PatientDoctors { get; set; } = new List<PatientDoctor>();
        public ICollection<Appointment> Appointments { get; set; }
    }
}
