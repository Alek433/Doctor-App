using System.ComponentModel.DataAnnotations;

namespace Doctor_App.Data.Models
{
    public class Doctor
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
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