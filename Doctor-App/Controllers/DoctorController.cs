using Doctor_App.Core.Models;
using Doctor_App.Core.Models.Doctor;
using Doctor_App.Core.Services;
using Doctor_App.Core.Services.DoctorServices;
using Doctor_App.Core.Services.PatientServices;
using Doctor_App.Data.Models;
using Doctor_App.Models.Doctor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using System.Security.Claims;

namespace Doctor_App.Controllers
{
    [Authorize]
    public class DoctorController : Controller
    {
        private readonly IDoctorService _doctorService;
        private readonly IMedicalRecordService _medicalRecordService;
        private readonly UserManager<IdentityUser> _userManager;

        public DoctorController(IDoctorService doctorService, IMedicalRecordService medicalRecordService, UserManager<IdentityUser> userManager)
        {
            _doctorService = doctorService;
            _medicalRecordService = medicalRecordService;
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
        public async Task<IActionResult> Dashboard()
        {

            string doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(doctorId))
            {
                return Unauthorized();
            }

            var patientRecords = await _medicalRecordService.GetPatientRecordsByDoctorIdAsync(doctorId);

            var viewModel = new DoctorDashboardViewModel
            {
                DoctorId = doctorId,
                PatientRecords = patientRecords
            };

            return View("Records/Dashboard", viewModel);
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View("Records/Add");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(PatientRecordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Get Doctor's ID (assuming User.Identity holds the doctor’s UserId)
            string doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(doctorId) || !Guid.TryParse(doctorId, out Guid doctorGuid))
            {
                return Unauthorized();
            }

            model.DoctorId = doctorGuid;
            await _medicalRecordService.AddPatientRecordAsync(model);

            return RedirectToAction("Dashboard");
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
            var record = await _medicalRecordService.GetPatientRecordAsync(id);
            if (record == null)
            {
                return NotFound();
            }
            return View(record);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]  
        public async Task<IActionResult> Edit(Guid id, PatientRecordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            bool updated = await _medicalRecordService.UpdatePatientRecordAsync(id, model);
            if (!updated)
            {
                return NotFound();
            }

            return RedirectToAction("ViewPatientRecords");
        }
        //public IActionResult EditMedicalCard(int patientId) => View();
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            bool deleted = await _medicalRecordService.DeletePatientRecordAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return RedirectToAction("ViewPatientRecords");
        }
    }
}
