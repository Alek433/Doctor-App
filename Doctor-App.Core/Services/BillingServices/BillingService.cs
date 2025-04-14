using Doctor_App.Core.Models.Billing;
using Doctor_App.Data.Models;
using Doctor_App.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
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
        public async Task CreateBillAsync(BillViewModel model)
        {
            var entity = new Billing
            {
                VisitId = model.VisitId,
                Amount = model.Amount,
                InsuranceDetails = model.InsuranceDetails,
                PaymentStatus = model.PaymentStatus
            };

            _dbContext.Billings.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<BillViewModel>> GetAllBillsAsync()
        {
            return await _dbContext.Billings
            .Select(b => new BillViewModel
            {
                Id = b.Id,
                VisitId = b.VisitId,
                Amount = b.Amount,
                InsuranceDetails = b.InsuranceDetails,
                PaymentStatus = b.PaymentStatus
            }).ToListAsync();
        }

        public async Task<BillViewModel?> GetBillByVisitIdAsync(int visitId)
        {
            var bill = await _dbContext.Billings
            .FirstOrDefaultAsync(b => b.VisitId == visitId);

            if (bill == null) return null;

            return new BillViewModel
            {
                Id = bill.Id,
                VisitId = bill.VisitId,
                Amount = bill.Amount,
                InsuranceDetails = bill.InsuranceDetails,
                PaymentStatus = bill.PaymentStatus
            };
        }

        public async Task UpdatePaymentStatusAsync(int billingId, string status)
        {
            var bill = await _dbContext.Billings.FindAsync(billingId);
            if (bill != null)
            {
                bill.PaymentStatus = status;
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task<BillViewModel?> GetBillByIdAsync(int billId)
        {
            var bill = await _dbContext.Billings
                .Include(b => b.Visit)
                .ThenInclude(v => v.Patient)
                .FirstOrDefaultAsync(b => b.Id == billId);

            if (bill == null) return null;

            return new BillViewModel
            {
                Id = bill.Id,
                VisitId = bill.VisitId,
                Amount = bill.Amount,
                InsuranceDetails = bill.InsuranceDetails,
                PaymentStatus = bill.PaymentStatus,
                PatientName = bill.Visit.Patient.FirstName + " " + bill.Visit.Patient.LastName
            };
        }

        public async Task UpdateBillAsync(BillViewModel model)
        {
            var bill = await _dbContext.Billings.FindAsync(model.Id);

            if (bill != null)
            {
                bill.Amount = model.Amount;
                bill.InsuranceDetails = model.InsuranceDetails;
                bill.PaymentStatus = model.PaymentStatus;

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
