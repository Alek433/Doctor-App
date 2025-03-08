using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor_App.Core.Models.Patient
{
    public class BecomePatientViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } 
        public string ContactInformation { get; set; }
        public string Address { get; set; }
        public string EmergencyContact { get; set; }
    }
}
