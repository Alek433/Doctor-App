using Doctor_App.Core.Extensions;
using Doctor_App.Core.Models.Billing;
using Doctor_App.Data.Models;
using Doctor_App.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor_App.Core.Services.BillingServices
{
    public class BillingService : IBillingService
    {
        private readonly DoctorAppDbContext _dbContext;

        public BillingService(DoctorAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<BillViewModel?> GetBillViewByVisitIdAsync(int visitId)
        {
            var visit = await _dbContext.Visits
                .Include(v => v.Doctor)
                .Include(v => v.Patient)
                .FirstOrDefaultAsync(v => v.Id == visitId);

            if (visit == null) return null;

            var amount = BillingPricingHelper.CalculateBillingAmount(visit.Diagnosis, visit.Doctor.Specialization);

            return new BillViewModel
            {
                VisitId = visit.Id,
                Amount = amount,
                DoctorName = $"{visit.Doctor.FirstName} {visit.Doctor.LastName}",
                PatientName = $"{visit.Patient.FirstName} {visit.Patient.LastName}",
                PaymentStatus = "SentToPatient"
            };
        }

        public async Task CreateBillAsync(BillViewModel model)
        {
            var bill = new Billing
            {
                VisitId = model.VisitId,
                Amount = model.Amount,
                InsuranceDetails = model.InsuranceDetails,
                PaymentStatus = model.PaymentStatus
            };

            _dbContext.Billings.Add(bill);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<BillViewModel>> GetAllBillsAsync()
        {
            var bills = await _dbContext.Billings
                .Include(b => b.Visit)
                    .ThenInclude(v => v.Patient)
                .Include(b => b.Visit)
                    .ThenInclude(v => v.Doctor)
                .ToListAsync();

            return bills.Select(b => new BillViewModel
            {
                Id = b.Id,
                VisitId = b.VisitId,
                Amount = b.Amount,
                VisitDate = b.Visit.VisitDate,
                InsuranceDetails = b.InsuranceDetails,
                PaymentStatus = "Paid",
                PatientName = $"{b.Visit.Patient.FirstName} {b.Visit.Patient.LastName}",
                DoctorName = $"{b.Visit.Doctor.FirstName} {b.Visit.Doctor.LastName}"
            }).ToList();
        }
        public async Task<IEnumerable<BillViewModel>> GetBillsPendingApprovalAsync()
        {
            return await _dbContext.Billings
                .Where(b => b.PaymentStatus == "PendingAdminReview")
                .Select(b => new BillViewModel
                {
                    Id = b.Id,
                    VisitId = b.VisitId,
                    Amount = b.Amount,
                    InsuranceDetails = b.InsuranceDetails,
                    PaymentStatus = b.PaymentStatus,
                    DoctorName = b.Visit.Doctor.FirstName + " " + b.Visit.Doctor.LastName,
                    PatientName = b.Visit.Patient.FirstName + " " + b.Visit.Patient.LastName
                })
                .ToListAsync();
        }
        public async Task UpdateBillAsync(BillViewModel model)
        {
            var bill = await _dbContext.Billings
                .Include(b => b.Visit)
                .ThenInclude(v => v.Patient)
                .FirstOrDefaultAsync(b => b.Id == model.Id);

            if (bill != null)
            {
                bill.Amount = model.Amount;
                bill.InsuranceDetails = model.InsuranceDetails;
                bill.PaymentStatus = model.PaymentStatus;

                await _dbContext.SaveChangesAsync();

            }
        }
        public async Task ApproveBillAsync(int billId)
        {
            var bill = await _dbContext.Billings
                .FindAsync(billId);
            if (bill != null)
            {
                bill.PaymentStatus = "Paid";
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<BillViewModel?> GetBillByIdAsync(int billId)
        {
            var bill = await _dbContext.Billings
                .Include(b => b.Visit)
                  .ThenInclude(v => v.Patient)
                .Include(b => b.Visit)
                  .ThenInclude(v => v.Doctor)
                .FirstOrDefaultAsync(b => b.Id == billId);

            if (bill == null)
                return null;

            return new BillViewModel
            {
                Id = bill.Id,
                VisitId = bill.VisitId,
                Amount = bill.Amount,
                InsuranceDetails = bill.InsuranceDetails,
                PaymentStatus = bill.PaymentStatus,
                PatientName = bill.Visit.Patient.FirstName + " " + bill.Visit.Patient.LastName,
                DoctorName = bill.Visit.Doctor.FirstName + " " + bill.Visit.Doctor.LastName
            };
        }

        public async Task<List<BillViewModel>> GetBillsByPatientUserIdAsync(string userId)
        {
            var bills = await _dbContext.Billings
                .Include(b => b.Visit)
                    .ThenInclude(v => v.Patient)
                .Include(b => b.Visit.Doctor)
                .Where(b => b.Visit.Patient.UserId == userId && b.PaymentStatus == "SentToPatient")
                .ToListAsync();

            return bills.Select(b => new BillViewModel
            {
                Id = b.Id,
                VisitId = b.VisitId,
                Amount = b.Amount,
                InsuranceDetails = b.InsuranceDetails,
                PaymentStatus = b.PaymentStatus,
                PatientName = b.Visit.Patient.FirstName + " " + b.Visit.Patient.LastName,
                DoctorName = b.Visit.Doctor.FirstName + " " + b.Visit.Doctor.LastName
            }).ToList();
        }
        public async Task<bool> DeleteBillByIdAsync(int id)
        {
            var bill = await _dbContext.Billings.FindAsync(id);
            if (bill == null)
                return false;

            _dbContext.Billings.Remove(bill);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<BillViewModel>> GetBillsForPatientAsync(Guid patientId)
        {
            return await _dbContext.Billings
                .Where(b => b.PaymentStatus == "SentToPatient" && b.Visit.PatientId == patientId)
                .Select(b => new BillViewModel
                {
                    Id = b.Id,
                    VisitId = b.VisitId,
                    Amount = b.Amount,
                    InsuranceDetails = b.InsuranceDetails,
                    PaymentStatus = b.PaymentStatus,
                    DoctorName = b.Visit.Doctor.FirstName + " " + b.Visit.Doctor.LastName,
                    PatientName = b.Visit.Patient.FirstName + " " + b.Visit.Patient.LastName
                })
                .ToListAsync();
        }
        public async Task<List<BillViewModel>> GetPaidBillsByDoctorAsync(string doctorId)
        {
            var bills = await _dbContext.Billings
                .Include(b => b.Visit)
                    .ThenInclude(v => v.Patient)
                .Where(b => b.Visit.Doctor.UserId == doctorId && b.PaymentStatus == "Paid")
                .Select(b => new BillViewModel
                {
                    Amount = b.Amount,
                    PatientName = b.Visit.Patient.FirstName + " " + b.Visit.Patient.LastName,
                    VisitDate = b.Visit.VisitDate,
                    PaymentStatus = b.PaymentStatus
                })
                .ToListAsync();

            return bills;
        }
        public async Task<List<BillViewModel>> GetAllApprovedBillsAsync()
        {
            var bills = await _dbContext.Billings
                .Include(b => b.Visit)
                    .ThenInclude(v => v.Patient)
                .Include(b => b.Visit)
                    .ThenInclude(v => v.Doctor)
                .Where(b => b.PaymentStatus == "Paid") // Only approved and paid
                .ToListAsync();

            return bills.Select(b => new BillViewModel
            {
                Id = b.Id,
                Amount = b.Amount,
                PatientName = b.Visit.Patient.FirstName + " " + b.Visit.Patient.LastName,
                DoctorName = b.Visit.Doctor.FirstName + " " + b.Visit.Doctor.LastName,
                VisitDate = b.Visit.VisitDate,
                PaymentStatus = b.PaymentStatus
            }).ToList();
        }
    }
}
