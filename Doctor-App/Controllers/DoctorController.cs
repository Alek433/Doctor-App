using Doctor_App.Core.Models.Doctor;
using Doctor_App.Core.Services.DoctorServices;
using Doctor_App.Core.Services.PatientServices;
using Doctor_App.Data.Models;
using Doctor_App.Infrastructure.Data.Entities;
using Doctor_App.Models.Doctor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Doctor_App.Core.Services.MedicalRecordServices;
using Doctor_App.Core.Models.Patient;

namespace Doctor_App.Controllers
{
    [Authorize]
    public class DoctorController : Controller
    {
        private readonly IDoctorService _doctorService;
        private readonly IMedicalRecordService _medicalRecordService;
        private readonly DoctorAppDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public DoctorController(IDoctorService doctorService, IMedicalRecordService medicalRecordService, DoctorAppDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _doctorService = doctorService;
            _medicalRecordService = medicalRecordService;
            _dbContext = dbContext;
            _userManager = userManager;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Become()
        {
            var userId = _userManager.GetUserId(User);

            if (await _doctorService.ExistsByIdAsync(userId!))
            {
                return RedirectToAction("Index", "Home"); // Redirect if already a doctor
            }
            return View(new BecomeDoctorModel());
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Become(BecomeDoctorModel model)
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
            if (await _doctorService.ExistsByIdAsync(userId))
            {
                return RedirectToAction("Index", "Home");
            }

            // Add doctor and get the generated ID
            var doctorId = await _doctorService.AddDoctorAsync(userId, model);

            if (doctorId == Guid.Empty) // If the doctor was not added correctly
            {
                ModelState.AddModelError("", "An error occurred while processing your request.");
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> MyPatients()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var doctor = await _dbContext.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
            if (doctor == null) 
                return Unauthorized();

            var patients = await _doctorService.GetPatientsByDoctorIdAsync(doctor.Id);
            return View(patients);
        }
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            string doctorUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(doctorUserId))
            {
                return Unauthorized();
            }

            var patientRecords = await _medicalRecordService.GetPatientRecordsByDoctorIdAsync(doctorUserId);
            var visitStats = await _medicalRecordService.GetVisitStatsAsync(doctorUserId);
            var visits = await _medicalRecordService.GetAllVisitsAsync();
            var billedVisitIds = await _dbContext.Billings
                     .Select(b => b.VisitId)
                     .ToListAsync();

            // Set the HasBilling flag on each patient record
            foreach (var record in patientRecords)
            {
                record.HasBilling = billedVisitIds.Contains(record.Id);
            }
            var viewModel = new DoctorDashboardViewModel
            {
                DoctorId = doctorUserId,
                PatientRecords = patientRecords,
                VisitStats = visitStats

            };

            return View("Records/Dashboard", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            string doctorUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var patients = await _medicalRecordService.GetPatientsForDoctorAsync(Guid.Parse(doctorUserId));
            if (string.IsNullOrEmpty(doctorUserId))
            {
                return Unauthorized();
            }

            var model = new PatientRecordViewModel
            {
                Patients = patients
            };

            return View("Records/Add", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(PatientRecordViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Pass the correct userId
            var visitId = await _medicalRecordService.AddPatientRecordAsync(model, userId);
            return RedirectToAction("Dashboard");

            //return RedirectToAction("Create", "Billing", new { visitId = visitId });
        }
        public async Task<IActionResult> ViewPatientRecords()
        {
            var records = await _medicalRecordService.GetAllPatientRecordsAsync();
            return View(records);
        }
        //public IActionResult ManageAppointments() => View();
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var record = await _medicalRecordService.GetPatientRecordByIdAsync(id);

            if (record == null)
            {
                return NotFound();
            }

            return View("Records/Edit", record);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]  
        public async Task<IActionResult> Edit(PatientRecordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Doctor/Records/Edit.cshtml", model); // ✅ Explicit view path
            }

            await _medicalRecordService.UpdatePatientRecordAsync(model);
            return RedirectToAction("Dashboard");
            /*if (!ModelState.IsValid)
            {
                return View("Records/Edit", model);
            }
            bool updated = await _medicalRecordService.UpdatePatientRecordAsync(model);
            if (!updated)
            {
                return NotFound();
            }

            return RedirectToAction("Dashboard");*/
        }
        //public IActionResult EditMedicalCard(int patientId) => View();
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var record = await _medicalRecordService.GetPatientRecordByIdAsync(id);
            if (record == null)
            {
                return NotFound();
            }

            await _medicalRecordService.DeletePatientRecordAsync(id);
            return RedirectToAction("Dashboard");
        }
        public async Task<IActionResult> VisitsByDate(DateTime date)
        {
            var visits = await _medicalRecordService.GetVisitsByDateAsync(date);
            return PartialView("_VisitListPartial", visits);
        }
        [HttpGet]
        public async Task<IActionResult> View(int id)
        {
            var record = await _medicalRecordService.GetPatientRecordByIdAsync(id);
            if (record == null)
            {
                return NotFound();
            }
            return View("Records/View", record);
        }
    }
}
