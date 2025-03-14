using Doctor_App.Core.Models.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor_App.Core.Models.Patient
{
    public class AssignDoctorViewModel
    {
        public List<DoctorViewModel> Doctors { get; set; } = new List<DoctorViewModel>();
    }
}
