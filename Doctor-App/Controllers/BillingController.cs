using Doctor_App.Infrastructure.Data;
using Doctor_App.Infrastructure.Data.Entities;
using Doctor_App.Core.Models.Billing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Doctor_App.Data.Models;
using Doctor_App.Core.Extensions;
using Doctor_App.Core.Services.BillingServices;
using Doctor_App.Core.Services.MedicalRecordServices;

namespace Doctor_App.Web.Controllers
{
    [Route("Billing")]
    public class BillingController : Controller
    {
        private readonly DoctorAppDbContext _context;
        private readonly IBillingService _billingService;
        private readonly IMedicalRecordService _medicalRecordService;

        public BillingController(DoctorAppDbContext context, IBillingService billingService, IMedicalRecordService medicalRecordService)
        {
            _context = context;
            _billingService = billingService;
            _medicalRecordService = medicalRecordService;
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create(int visitId)
        {
            var model = await _billingService.GetBillViewByVisitIdAsync(visitId);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(BillViewModel model)
        {
            //if (!ModelState.IsValid) return View(model);

            await _billingService.CreateBillAsync(model);
            return RedirectToAction("Dashboard", "Doctor");
        }
        /*[HttpGet]
        public async Task<IActionResult> EditBill(int id)
        {
            var bill = await _billingService.GetBillByIdAsync(id);
            if (bill == null)
            {
                return NotFound();
            }

            return View("EditBill", bill); // View: Edit.cshtml
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBill(int id, BillViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            await _billingService.UpdateBillAsync(model);

            // Optionally: notify patient (via email/notification system)

            return RedirectToAction("MyBills", "Patient"); // Or any admin dashboard
        }*/


        [HttpGet("Review")]
        public async Task<IActionResult> Review()
        {
            var bills = await _billingService.GetPendingBillsAsync();
            return View(bills);
        }

        /*[HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var bill = await _billingService.GetBillByIdAsync(id);
            if (bill == null) return NotFound();

            bill.PaymentStatus = "SentToPatient";
            await _billingService.UpdateBillAsync(bill); // make sure this method exists
            return RedirectToAction("Edit");
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(int id)
        {
            await _billingService.MarkAsPaidAsync(id);
            return RedirectToAction("MyBills", "Patient");
        }*/
    }
}