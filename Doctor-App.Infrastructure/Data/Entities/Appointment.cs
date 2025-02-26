using System.ComponentModel.DataAnnotations;

namespace Doctor_App.Infrastructure.Data.Entities
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } // e.g., Scheduled, Completed, Canceled

        // Navigation Properties
        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
    }
}
