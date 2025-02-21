using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace Doctor_App.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class DoctorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AddMedicalRecord() => View();
        public IActionResult ManageAppointments() => View();
        public IActionResult EditMedicalCard(int patientId) => View();
        public IActionResult DeleteRecord(int recordId) => View();
    }
}
