using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor_App.Core.Models.Doctor
{
    public class AddPatientRecordModel
    {
        [Required]
        public Guid PatientId { get; set; } // Select from existing patients
        [Required]
        public string Diagnosis { get; set; }
        [Required]
        public string Treatment { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime VisitDate { get; set; } = DateTime.Now;
        public string ReasonForVisit { get; set; }
        public string Prescriptions { get; set; }
        public string Notes { get; set; }
        public List<SelectListItem> Patients { get; set; } = new();
    }
}
