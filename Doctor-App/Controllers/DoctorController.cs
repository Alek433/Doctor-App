using Doctor_App.Core.Services.DoctorServices;
using Doctor_App.Data.Models;
using Doctor_App.Models.Doctor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace Doctor_App.Controllers
{
    [Authorize]
    public class DoctorController : Controller
    {
        private readonly IBecomeDoctorService _becomeDoctorService;
        private readonly UserManager<IdentityUser> _userManager;

        public DoctorController(IBecomeDoctorService becomeDoctorService, UserManager<IdentityUser> userManager)
        {
            _becomeDoctorService = becomeDoctorService;
            _userManager = userManager;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Become()
        {
            var userId = _userManager.GetUserId(User);

            if (await _becomeDoctorService.ExistsByIdAsync(userId!))
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
            if (await _becomeDoctorService.ExistsByIdAsync(userId))
            {
                return RedirectToAction("Index", "Home");
            }

            // Add doctor and get the generated ID
            var doctorId = await _becomeDoctorService.AddDoctorAsync(userId, model);

            if (doctorId == Guid.Empty) // If the doctor was not added correctly
            {
                ModelState.AddModelError("", "An error occurred while processing your request.");
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }
        public IActionResult AddMedicalRecord()
        {
           return View();
        }
        public IActionResult ManageAppointments() => View();
        public IActionResult EditMedicalCard(int patientId) => View();
        public IActionResult DeleteRecord(int recordId) => View();
    }
}
