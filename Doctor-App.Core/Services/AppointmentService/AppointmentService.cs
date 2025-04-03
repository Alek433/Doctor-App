using Doctor_App.Core.Models.Appointment;
using Doctor_App.Data.Models;
using Doctor_App.Infrastructure.Data.Entities;
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
            var appointment = new Appointment
            {
                PatientId = model.PatientId,
                DoctorId = model.DoctorId,
                AppointmentDate = model.AppointmentDate,
                Reason = model.Reason,
                Status = model.Status ?? "Scheduled"
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return true;
        }

        // ✅ Get all appointments
        public async Task<List<AppointmentViewModel>> GetAllAppointmentsAsync()
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .OrderBy(a => a.AppointmentDate)
                .Select(a => new AppointmentViewModel
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    DoctorId = a.DoctorId,
                    AppointmentDate = a.AppointmentDate,
                    Reason = a.Reason,
                    Status = a.Status
                })
                .ToListAsync();
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
                    Status = a.Status
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
                    Status = a.Status
                })
                .ToListAsync();
        }

        // ✅ Cancel an appointment
        public async Task<bool> CancelAppointmentAsync(int appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null) return false;

            appointment.Status = "Cancelled";
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
