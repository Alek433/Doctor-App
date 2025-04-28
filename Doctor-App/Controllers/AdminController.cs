using Doctor_App.Core.Models.Billing;
using Doctor_App.Core.Services.AppointmentService;
using Doctor_App.Core.Services.BillingServices;
using Doctor_App.Core.Services.DoctorServices;
using Doctor_App.Core.Services.MedicalRecordServices;
using Doctor_App.Core.Services.PatientServices;
using Doctor_App.Infrastructure.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Doctor_App.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private IBillingService _billingService;
        private IDoctorService _doctorService;
        private IPatientService _patientService;
        private IMedicalRecordService _medicalRecordService;
        private IAppointmentService _appointmentService;
        
        public AdminController(IBillingService billingService, IDoctorService doctorService, IPatientService patientService, IMedicalRecordService medicalRecordService, IAppointmentService appointmentService)
        {
            _billingService = billingService;
            _doctorService = doctorService;
            _patientService = patientService;
            _medicalRecordService = medicalRecordService;
            _appointmentService = appointmentService;
        }
        // View all bills
        public async Task<IActionResult> AllBills()
        {
            var bills = await _billingService.GetAllBillsAsync();
            return View(bills);
        }
        [HttpGet]
        public async Task<IActionResult> PendingBills()
        {
            var pending = (await _billingService.GetBillsPendingApprovalAsync())
               .Where(b => b.PaymentStatus == "PendingAdminReview");

            return View(pending);
        }
        [HttpGet]
        public async Task<IActionResult> Review(int id)
        {
            var bill = await _billingService.GetBillByIdAsync(id);
            if (bill == null) return NotFound();
            return View(bill);
        }

        [HttpPost]
        public async Task<IActionResult> Review(BillViewModel model, string action)
        {
            if (action == "Approve")
            {
                model.PaymentStatus = "Paid";
            }
            else if (action == "Reject")
            {
                model.PaymentStatus = "ReturnedToPatient";
            }

            await _billingService.UpdateBillAsync(model);
            return RedirectToAction("PendingBills");
        }
        // Manage doctors
        public async Task<IActionResult> ManageDoctors()
        {
            var doctors = await _doctorService.GetAllDoctorsAsync();
            return View(doctors);
        }
        [HttpPost]
        public async Task<IActionResult> ApproveDoctor(Guid id)
        {
            var success = await _doctorService.ApproveDoctorAsync(id);
            if (!success) return NotFound();

            return RedirectToAction(nameof(ManageDoctors));
        }
        [HttpPost]
        public async Task<IActionResult> DeleteDoctor(Guid id)
        {
            var success = await _doctorService.DeleteDoctorAsync(id);
            if (!success) return NotFound();

            return RedirectToAction(nameof(ManageDoctors));
        }

        // Manage patients
        public async Task<IActionResult> ManagePatients()
        {
            var patients = await _patientService.GetAllPatientsAsync();
            return View(patients);
        }
        public async Task<IActionResult> DeleteBill(int id)
        {
            var result = await _billingService.DeleteBillByIdAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return RedirectToAction("PendingBills");
        }

        // View all medical records
        public async Task<IActionResult> AllRecords()
        {
            var records = await _medicalRecordService.GetAllPatientRecordsAsync();
            return View(records);
        }
        public async Task<IActionResult> AdminAppointments()
        {
            var appointments = await _appointmentService.GetAllAppointmentsAsync(); // Implement this in the service
            return View(appointments);
        }
        [HttpGet]
        public async Task<IActionResult> AllApprovedBills()
        {
            var bills = await _billingService.GetAllApprovedBillsAsync();
            return View(bills);
        }
    }
}
