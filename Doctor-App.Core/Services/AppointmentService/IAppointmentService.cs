﻿using Doctor_App.Core.Models.Appointment;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor_App.Core.Services.AppointmentService
{
    public interface IAppointmentService
    {
        Task<bool> CreateAppointmentAsync(AppointmentViewModel model);
        Task<List<AppointmentViewModel>> GetAllAppointmentsAsync();
        Task<List<AppointmentViewModel>> GetAppointmentsByPatientAsync(Guid patientId);
        Task<List<AppointmentViewModel>> GetAppointmentsByDoctorAsync(Guid doctorId);
        Task CancelAppointmentAsync(int appointmentId);
        Task<List<SelectListItem>> GetAvailableDoctorsAsync();
        Task<List<SelectListItem>> GetAvailablePatientsAsync();
    }
}
