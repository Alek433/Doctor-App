using Doctor_App.Core.Models;
using Doctor_App.Core.Models.Doctor;
using Doctor_App.Core.Models.Patient;
using Doctor_App.Core.Services;
using Doctor_App.Core.Services.DoctorServices;
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
        private readonly IDoctorService _doctorService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly DoctorAppDbContext _context;

        public PatientController(IPatientService patientService, IMedicalRecordService medicalRecordService, IDoctorService doctorService, DoctorAppDbContext context, UserManager<IdentityUser> userManager)
        {
            _patientService = patientService;
            _medicalRecordService = medicalRecordService;
            _doctorService = doctorService;
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
        [HttpGet]
        public async Task<IActionResult> ViewMyRecords()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Ensure you have a method to get PatientId from UserId
            var patient = await _patientService.GetPatientByUserIdAsync(userId);

            if (patient == null)
            {
                return NotFound("Patient record not found.");
            }

            Guid patientId = patient.Id; // Get the PatientId from the retrieved patient

            var records = await _medicalRecordService.GetPatientRecordsByPatientIdAsync(patientId);

            if (records == null || !records.Any())
            {
                return View(); // Show a friendly "No records found" view if necessary
            }

            return View("ViewMyRecords", records);
        }
        [HttpGet]
        public async Task<IActionResult> AssignDoctor()
        {
            var patientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (patientId == null)
                return Unauthorized();

            // Retrieve the patient from the database
            var patient = await _context.FindAsync<Patient>(Guid.Parse(patientId));

            // Retrieve all doctors from the database
            var doctors = await _context.Doctors.ToListAsync();

            // Convert the doctors to a ViewModel list
            var doctorList = doctors.Select(d => new DoctorViewModel
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                Specialization = d.Specialization
            }).ToList();

            return View(doctorList);
        }
        [HttpPost]
        public async Task<IActionResult> AssignDoctor(Guid doctorId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the logged-in user's ID

            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null)
            {
                return NotFound("Patient not found.");
            }
            await _patientService.AssignDoctorToPatientAsync(patient.Id, doctorId);

            return RedirectToAction("Index", "Home");
        }
        public IActionResult ReceiveNotifications() => View();
    }
}
