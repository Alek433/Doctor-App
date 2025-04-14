using Doctor_App.Core.Models.Billing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor_App.Core.Services.BillingServices
{
    public interface IBillingService
    {
        Task<List<BillViewModel>> GetAllBillsAsync();
        Task<BillViewModel?> GetBillByVisitIdAsync(int visitId);
        Task CreateBillAsync(BillViewModel model);
        Task UpdatePaymentStatusAsync(int billingId, string status);
        Task<BillViewModel?> GetBillByIdAsync(int billId);
        Task UpdateBillAsync(BillViewModel model);
    }
}
