using Doctor_App.Core.Models.Patient;
using Doctor_App.Core.Services;
using Doctor_App.Core.Services.PatientServices;
using Doctor_App.Data.Models;
using Doctor_App.Infrastructure.Data.Entities;
using Doctor_App.Models.Doctor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Doctor_App.Controllers
{
    [Authorize]
    public class PatientController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly IMedicalRecordService _medicalRecordService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly DoctorAppDbContext _context;

        public PatientController(IPatientService patientService, IMedicalRecordService medicalRecordService, DoctorAppDbContext context, UserManager<IdentityUser> userManager)
        {
            _patientService = patientService;
            _medicalRecordService = medicalRecordService;
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Become()
        {
            var userId = _userManager.GetUserId(User);

            if (await _patientService.ExistsByIdAsync(userId!))
            {
                return RedirectToAction("Index", "Home"); // Redirect if already a patient
            }
            return View(new BecomePatientViewModel());
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Become(BecomePatientViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized(); // Ensure the user is logged in
            }

            // Check if the user is already a doctor before adding
            if (await _patientService.ExistsByIdAsync(userId))
            {
                return RedirectToAction("Index", "Home");
            }

            // Add doctor and get the generated ID
            var patientId = await _patientService.AddPatientAsync(userId, model);

            if (patientId == Guid.Empty) // If the doctor was not added correctly
            {
                ModelState.AddModelError("", "An error occurred while processing your request.");
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> ViewMyRecords()
        {
            int patientId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (string.IsNullOrEmpty(patientId.ToString()))
            {
                return Unauthorized();
            }

            var records = await _medicalRecordService.GetPatientRecordAsync(patientId);
            return View(records);
        }
        [HttpPost]
        public async Task<IActionResult> AssignDoctor(Guid doctorId)
        {
            var patientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (patientId == null) return Unauthorized();

            var relationship = new Appointment { DoctorId = doctorId, PatientId = Guid.Parse(patientId) };
            _context.Appointments.Add(relationship);
            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard", "Patient");
        }
        public IActionResult ReceiveNotifications() => View();
    }
}
