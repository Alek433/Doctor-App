using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor_App.Core.Models.Visit
{
    public class VisitDetailViewModel
    {
        public string PatientName { get; set; }
        public string ReasonForVisit { get; set; }
        public string Diagnosis { get; set; }
        public string Prescriptions { get; set; }
        public string Notes { get; set; }
    }
}
