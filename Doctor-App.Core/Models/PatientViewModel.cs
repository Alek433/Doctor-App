using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor_App.Core.Models
{
    public class PatientViewModel
    {
        public Guid Id { get; set; }
        public string FirstName {  get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender {  get; set; }
        public string ContactInformation {  get; set; }
    }
}
