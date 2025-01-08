using System.ComponentModel.DataAnnotations;

namespace Doctor_App.Data.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int PatientId { get; set; }
        [Required]
        public int DoctorId { get; set; }
        [Required]
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } // e.g., Scheduled, Completed, Canceled

        // Navigation Properties
        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
    }
}
