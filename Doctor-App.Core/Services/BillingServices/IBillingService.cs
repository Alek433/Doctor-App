using Doctor_App.Core.Models.Billing;
using Doctor_App.Infrastructure.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor_App.Core.Services.BillingServices
{
    public interface IBillingService
    {
        Task<BillViewModel?> GetBillViewByVisitIdAsync(int visitId);
        Task CreateBillAsync(BillViewModel model);
        Task<List<BillViewModel>> GetAllBillsAsync();
        Task<IEnumerable<BillViewModel>> GetBillsPendingApprovalAsync();
        Task ApproveBillAsync(int billId);
        Task<BillViewModel?> GetBillByIdAsync(int billId);
        Task UpdateBillAsync(BillViewModel model);
        Task<List<BillViewModel>> GetBillsByPatientUserIdAsync(string userId);
        Task<IEnumerable<BillViewModel>> GetBillsForPatientAsync(Guid patientId);
        Task<bool> DeleteBillByIdAsync(int id);
        Task<List<BillViewModel>> GetPaidBillsByDoctorAsync(string doctorId);
        Task<List<BillViewModel>> GetAllApprovedBillsAsync();
    }
}
