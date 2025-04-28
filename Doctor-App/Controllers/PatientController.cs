using Doctor_App.Core.Extensions;
using Doctor_App.Core.Models;
using Doctor_App.Core.Models.Billing;
using Doctor_App.Core.Models.Doctor;
using Doctor_App.Core.Models.Patient;
using Doctor_App.Core.Services.BillingServices;
using Doctor_App.Core.Services.DoctorServices;
using Doctor_App.Core.Services.MedicalRecordServices;
using Doctor_App.Core.Services.PatientServices;
using Doctor_App.Data.Models;
using Doctor_App.Infrastructure.Data.Entities;
using Doctor_App.Models.Doctor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Web.WebPages.Html;

namespace Doctor_App.Controllers
{
    [Authorize]
    public class PatientController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly IMedicalRecordService _medicalRecordService;
        private readonly IDoctorService _doctorService;
        private readonly IBillingService _billingService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly DoctorAppDbContext _context;

        public PatientController(IPatientService patientService, IMedicalRecordService medicalRecordService, IDoctorService doctorService, IBillingService billingService, DoctorAppDbContext context, UserManager<IdentityUser> userManager)
        {
            _patientService = patientService;
            _medicalRecordService = medicalRecordService;
            _doctorService = doctorService;
            _billingService = billingService;
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

            return Redirect("/Identity/Account/Logout");
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
                return View("ViewMyRecords"); // Show a friendly "No records found" view if necessary
            }

            return View("ViewMyRecords", records);
        }
        [HttpGet]
        public async Task<IActionResult> AssignDoctor(string specialization, string city)
        {
            var patientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (patientId == null)
                return Unauthorized();

            var allDoctors = await _doctorService.GetAllDoctorsAsync();
            var assignedDoctorIds = await _patientService.GetAssignedDoctorIdsAsync(Guid.Parse(patientId));

            if (!string.IsNullOrWhiteSpace(specialization))
            {
                allDoctors = allDoctors
                    .Where(d => d.Specialization != null && d.Specialization.Contains(specialization, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                allDoctors = allDoctors
                    .Where(d => d.City != null && d.City.Contains(city, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            var model = allDoctors.Select(d => new AssignedDoctorViewModel
            {
                Doctor = d,
                IsAssigned = assignedDoctorIds.Contains(d.Id)
            }).ToList();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AssignDoctor(Guid doctorId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the logged-in user's ID
            
            //var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            var patient = await _patientService.GetPatientByUserIdAsync(userId);
            if (patient == null)
            {
                return NotFound("Doctor not found.");
            }
            if (await _patientService.IsAlreadyAssigned(patient.Id.ToString(), doctorId.ToString()))
            {
                return RedirectToAction("AssignDoctor");
            }
            await _patientService.AssignDoctorToPatientAsync(patient.Id, doctorId);

            return RedirectToAction("Index", "Home");
        }
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> MyBills()
        {
            var userId = User.GetUserId(); // Extension method or however you fetch current user ID
            var bills = await _billingService.GetBillsByPatientUserIdAsync(userId);

            return View(bills);
        }
        [HttpGet]
        public async Task<IActionResult> EditBill(int id)
        {
            var bill = await _billingService.GetBillByIdAsync(id);
            if (bill == null) return NotFound();

            return View(bill);
        }

        [HttpPost]
        public async Task<IActionResult> EditBill(BillViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            model.PaymentStatus = "PendingAdminReview"; // Next stage
            await _billingService.UpdateBillAsync(model);

            return RedirectToAction("MyBills");
        }

        [HttpGet]
        public async Task<IActionResult> MyDoctor()
        {
            var patientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (patientId == null)
                return Unauthorized();

            var doctor = await _patientService.GetAssignedDoctorAsync(patientId);

            if (doctor == null)
                return View("NoDoctorAssigned"); // Optional: view if no doctor assigned yet

            return View(doctor);
        }

        [HttpGet]
        public IActionResult RateDoctor(Guid doctorId)
        {
            var model = new RateDoctorViewModel
            {
                DoctorId = doctorId
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitRating(RateDoctorViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patient = await _patientService.GetPatientByUserIdAsync(userId);

            var patientDoctor = await _context.PatientDoctors
                .FirstOrDefaultAsync(pd => pd.PatientId == patient.Id && pd.DoctorId == model.DoctorId);

            if (patientDoctor == null)
            {
                return NotFound();
            }

            patientDoctor.Rating = model.Rating;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
        public IActionResult ReceiveNotifications() => View();
    }
}
