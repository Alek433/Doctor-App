using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor_App.Core.Models.Patient
{
    public class PatientRecordViewModel
    {
        public int Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime VisitDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ReasonForVisit { get; set; }
        public string Diagnosis { get; set; }
        public string Prescriptions { get; set; }
        public string Notes { get; set; }
        public bool HasBilling { get; set; }
        public List<SelectListItem> Patients { get; set; } = new List<SelectListItem>();

    }
}
