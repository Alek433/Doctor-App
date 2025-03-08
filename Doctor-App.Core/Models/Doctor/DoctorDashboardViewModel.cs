using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor_App.Core.Models.Doctor
{
    public class DoctorDashboardViewModel
    {
        public string DoctorId { get; set; }
        public List<PatientRecordViewModel> PatientRecords { get; set; } = new List<PatientRecordViewModel>();
        public AddPatientRecordModel NewPatientRecord { get; set; } = new AddPatientRecordModel();
    }
}
