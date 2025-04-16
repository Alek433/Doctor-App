using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor_App.Core.Models.Appointment
{
    public class ManageAppointmentsViewModel
    {
        public List<AppointmentViewModel> Appointments { get; set; } = new List<AppointmentViewModel>();

        public List<SelectListItem> AvailableDoctors { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> AvailablePatients { get; set; } = new List<SelectListItem>();
    }
}
