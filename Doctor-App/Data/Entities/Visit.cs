using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doctor_App.Data.Models
{
    public class Visit
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Id))]
        public int PatientId { get; set; }
        [ForeignKey(nameof(PatientId))]
        public int DoctorId { get; set; }

        public DateTime VisitDate { get; set; }
        public string ReasonForVisit { get; set; }
        public string Diagnosis { get; set; }
        public string Prescriptions { get; set; }
        public string Notes { get; set; }

        // Navigation Properties
        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
    }
}
