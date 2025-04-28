using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor_App.Core.Models.Doctor
{
    public class AssignedDoctorViewModel
    {
        public DoctorViewModel Doctor { get; set; }
        public bool IsAssigned { get; set; }
    }
}
