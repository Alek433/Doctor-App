using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor_App.Core.Models.Appointment
{
    public class CreateAppointmentViewModel
    {
        public int Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Reason { get; set; }
        public string Status {  get; set; }

        public List<SelectListItem> AvailableDoctors { get; set; } = new List<SelectListItem>();

    }
}
