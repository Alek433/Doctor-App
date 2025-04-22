using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor_App.Core.Models.Doctor
{
    public class DoctorViewModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Specialization {  get; set; }
        public string City { get; set; }
        public string OfficeLocation {  get; set; }
        public string ContactInformation {  get; set; }
    }
}
