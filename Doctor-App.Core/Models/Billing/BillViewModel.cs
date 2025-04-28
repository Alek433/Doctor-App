using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor_App.Core.Models.Billing
{
    public class BillViewModel
    {
        public int Id { get; set; }
        public int VisitId { get; set; }

        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [Display(Name = "Insurance Details")]
        public string InsuranceDetails { get; set; }

        [Display(Name = "Payment Status")]
        public string PaymentStatus { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public DateTime VisitDate { get; set; }
    }
}
