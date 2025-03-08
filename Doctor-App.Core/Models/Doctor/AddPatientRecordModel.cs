using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor_App.Core.Models.Doctor
{
    public class AddPatientRecordModel
    {
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; } // Select from existing patients
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public DateTime VisitDate { get; set; } = DateTime.Now;
        public string ReasonForVisit { get; set; }
        public string Prescriptions { get; set; }
        public string Notes { get; set; }
    }
}
