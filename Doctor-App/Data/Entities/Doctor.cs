using System.ComponentModel.DataAnnotations;

namespace Doctor_App.Data.Models
{
    public class Doctor
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }
        [Required]
        [MaxLength(100)]
        public string Specialization { get; set; }
        [Required]
        [MaxLength(100)]
        public string ContactInformation { get; set; }
        [Required]
        [MaxLength(100)]
        public string OfficeLocation { get; set; }

        // Navigation Property
        public ICollection<Visit> Visits { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }
}