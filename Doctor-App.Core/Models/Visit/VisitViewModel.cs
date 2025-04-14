using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor_App.Core.Models.Visit
{
    public class VisitViewModel
    {
        public int VisitId { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public DateTime VisitDate { get; set; }
        public string Diagnosis {  get; set; }
        public string Prescriptions { get; set; }
        public string ReasonForVisit { get; set; }
        public string Notes {  get; set; }
    }
}
