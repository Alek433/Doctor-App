using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doctor_App.Infrastructure.Data.Entities
{
    public class Doctor
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string UserId { get; set; } // Foreign key to Identity User

        [ForeignKey(nameof(UserId))]
        public virtual IdentityUser User { get; set; }
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }
        public string Specialization { get; set; }
        public string ContactInformation { get; set; }
        public string OfficeLocation { get; set; }

        // Navigation Property
        public ICollection<Visit> Visits { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }
}