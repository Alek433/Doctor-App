using Doctor_App.Core.Models.Appointment;
using Doctor_App.Data.Models;
using Doctor_App.Infrastructure.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor_App.Core.Services.AppointmentService
{
    public class AppointmentService : IAppointmentService
    {
        private readonly DoctorAppDbContext _context;

        public AppointmentService(DoctorAppDbContext context)
        {
            _context = context;
        }

        // ✅ Create an appointment
        public async Task<bool> CreateAppointmentAsync(AppointmentViewModel model)
        {
            if (model == null)
            {
                return false;
            }
            var patientExists = await _context.Patients.AnyAsync(p => p.Id == model.PatientId);
            if (!patientExists)
            {
                throw new InvalidOperationException("The patient with the provided ID does not exist.");
            }
            var doctorExists = await _context.Patients.AnyAsync(p => p.Id == model.DoctorId);
            if (!patientExists)
            {
                throw new InvalidOperationException("The doctor with the provided ID does not exist.");
            }
            var appointment = new Appointment
            {
                PatientId = model.PatientId,
                DoctorId = model.DoctorId,
                AppointmentDate = model.AppointmentDate,
                Reason = model.Reason,
                Status = "Scheduled"
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return true;
        }

        // ✅ Get all appointments
        public async Task<List<AppointmentViewModel>> GetAllAppointmentsAsync()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .ToListAsync();

            var result = appointments.Select(a => new AppointmentViewModel
            {
                AppointmentDate = a.AppointmentDate,
                Reason = a.Reason,
                PatientId = a.PatientId,
                DoctorId = a.DoctorId,
                PatientName = a.Patient != null ? a.Patient.FirstName + " " + a.Patient.LastName : "N/A",
                DoctorName = a.Doctor != null ? a.Doctor.FirstName + " " + a.Doctor.LastName : "N/A"
            }).ToList();

            return result;
        }

        // ✅ Get appointments by patient ID
        public async Task<List<AppointmentViewModel>> GetAppointmentsByPatientAsync(Guid patientId)
        {
            return await _context.Appointments
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.AppointmentDate)
                .Select(a => new AppointmentViewModel
                {
                    Id = a.Id,
                    DoctorId = a.DoctorId,
                    AppointmentDate = a.AppointmentDate,
                    Reason = a.Reason,
                })
                .ToListAsync();
        }

        // ✅ Get appointments by doctor ID
        public async Task<List<AppointmentViewModel>> GetAppointmentsByDoctorAsync(Guid doctorId)
        {
            return await _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .OrderByDescending(a => a.AppointmentDate)
                .Select(a => new AppointmentViewModel
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    AppointmentDate = a.AppointmentDate,
                    Reason = a.Reason,
                })
                .ToListAsync();
        }

        // ✅ Cancel an appointment
        public async Task CancelAppointmentAsync(int appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
                throw new InvalidOperationException("Appointment not found.");

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
        }
        public async Task<List<SelectListItem>> GetAvailableDoctorsAsync()
        {
            return await _context.Doctors
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.FirstName + " " + d.LastName
                })
                .ToListAsync();
        }
        public async Task<List<SelectListItem>> GetAvailablePatientsAsync()
        {
            return await _context.Patients
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.FirstName + " " + d.LastName
                })
                .ToListAsync();
        }
    }
}
