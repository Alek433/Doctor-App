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
        [Display(Name = "Visit ID")]
        public int VisitId { get; set; }

        [Display(Name = "Amount")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Display(Name = "Insurance Details")]
        public string InsuranceDetails { get; set; }

        [Display(Name = "Payment Status")]
        public string PaymentStatus { get; set; }

        [Display(Name = "Patient Name")]
        public string PatientName { get; set; }
    }
}
