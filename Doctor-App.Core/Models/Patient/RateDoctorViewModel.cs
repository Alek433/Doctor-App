using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor_App.Core.Models.Patient
{
    public class RateDoctorViewModel
    {
        public Guid DoctorId { get; set; }
        public int Rating { get; set; }
    }
}
