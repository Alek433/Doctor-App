using Doctor_App.Core.Models.Appointment;
using Doctor_App.Core.Services.AppointmentService;
using Doctor_App.Data.Models;
using Doctor_App.Infrastructure.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace Doctor_App.Controllers
{
    [Route("Appointment")]
    public class AppointmentController : Controller
    {
        private readonly DoctorAppDbContext _context;
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(DoctorAppDbContext context, IAppointmentService appointmentService)
        {
            _context = context;
            _appointmentService = appointmentService;
        }

        [HttpGet("Manage")]
        [Authorize(Roles = "Patient, Doctor")]
        public async Task<IActionResult> Manage()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var viewModel = new ManageAppointmentsViewModel();

            if (userRole == "Patient")
            {
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
                if (patient != null)
                {
                    viewModel.Appointments = await _context.Appointments
                        .Where(a => a.PatientId == patient.Id)
                        .Include(a => a.Doctor)
                        .Select(a => new AppointmentViewModel  // ✅ Convert Appointment to ViewModel
                        {
                            DoctorName = a.Doctor != null ? a.Doctor.FirstName + " " + a.Doctor.LastName : "N/A",
                            PatientId = a.PatientId,
                            DoctorId = a.DoctorId,
                            AppointmentDate = a.AppointmentDate,
                            Reason = a.Reason,
                        })
                        .ToListAsync();
                }
            }
            else if (userRole == "Doctor")
            {
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
                if (doctor != null)
                {
                    viewModel.Appointments = await _context.Appointments
                        .Where(a => a.DoctorId == doctor.Id)
                        .Include(a => a.Patient)
                        .Select(a => new AppointmentViewModel
                        {
                            PatientName = a.Patient != null ? a.Patient.FirstName + " " + a.Patient.LastName : "N/A",
                            PatientId = a.PatientId,
                            DoctorId = a.DoctorId,
                            AppointmentDate = a.AppointmentDate,
                            Reason = a.Reason,
                        })
                        .ToListAsync();
                }
            }

            return View("Manage", viewModel);
        }

        [HttpGet("Create")]
        [Authorize(Roles = "Patient,Doctor")] // Allows both roles to access
        public async Task<IActionResult> Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var viewModel = new AppointmentViewModel();

            if (userRole == "Patient")
            {
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
                if (patient == null) return Unauthorized();
                viewModel.PatientId = patient.Id;
            }
            else if (userRole == "Doctor")
            {
                // Load available patients for doctors to select from
                viewModel.AvailablePatients = await _context.Patients
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.FirstName + " " + p.LastName
                    })
                    .ToListAsync();
            }

            viewModel.AvailableDoctors = await _context.Doctors
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.FirstName + " " + d.LastName
                })
                .ToListAsync();

            return View("Create", viewModel); // ✅ Ensure passing the correct mo
        }

        [HttpPost("Create")]
        [Authorize(Roles = "Patient,Doctor")]
        public async Task<IActionResult> Create(AppointmentViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (role == "Patient")
            {
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
                if (patient == null)
                {
                    ModelState.AddModelError("", "You are not registered as a patient.");
                    model.AvailableDoctors = await _appointmentService.GetAvailableDoctorsAsync();
                    return View(model);
                }

                model.PatientId = patient.Id;
            }
            else if (role == "Doctor")
            {
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
                if (doctor == null)
                {
                    ModelState.AddModelError("", "You are not registered as a doctor.");
                    model.AvailableDoctors = await _appointmentService.GetAvailableDoctorsAsync();
                    model.AvailablePatients = await _appointmentService.GetAvailablePatientsAsync(); // load patients if doctor
                    return View(model);
                }

                model.DoctorId = doctor.Id;
            }

            if (!ModelState.IsValid)
            {
                model.AvailableDoctors = await _appointmentService.GetAvailableDoctorsAsync();
                if (role == "Doctor")
                {
                    model.AvailablePatients = await _appointmentService.GetAvailablePatientsAsync();
                }
                return View(model);
            }

            try
            {
                bool success = await _appointmentService.CreateAppointmentAsync(model);
                if (success)
                {
                    if (role == "Patient")
                    {
                        return RedirectToAction("Manage", "Appointment"); // redirect patients
                    }
                    else if (role == "Doctor")
                    {
                        return RedirectToAction("Manage", "Appointment"); // redirect doctors
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            model.AvailableDoctors = await _appointmentService.GetAvailableDoctorsAsync();
            if (role == "Doctor")
            {
                model.AvailablePatients = await _appointmentService.GetAvailablePatientsAsync();
            }

            return View(model);
        }

        [HttpPost("Cancel/{appointmentId}")]
        [Authorize(Roles = "Patient,Doctor")]
        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            bool success = await _appointmentService.CancelAppointmentAsync(appointmentId);
            if (!success)
            {
                ModelState.AddModelError("", "Failed to cancel appointment.");
            }

            return RedirectToAction("Manage");
        }
    }
}
