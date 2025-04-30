using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor_App.Core.Models.Appointment
{
    public class AppointmentViewModel
    {
        public int Id { get; set; }
        public Guid PatientId { get; set; }  // For doctor selection
        public Guid DoctorId { get; set; }   // For patient selection
        public DateTime AppointmentDate { get; set; }
        public string Reason { get; set; }

        public string? PatientName { get; set; }  // For doctor view
        public string? DoctorName { get; set; }   // For patient view
        public List<SelectListItem> AvailableDoctors { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> AvailablePatients { get; set; } = new List<SelectListItem>();
    }
}
