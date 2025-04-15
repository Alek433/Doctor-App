using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor_App.Core.Extensions
{
    public class BillingPricingHelper
    {
        private static readonly Dictionary<string, decimal> DiagnosisPrices = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Flu", 50.00m },
            { "Checkup", 75.00m },
            { "X-Ray", 120.00m },
            { "Diabetes", 100.00m },
            { "Hypertension", 90.00m },
            { "Other", 60.00m }
        };

        private static readonly Dictionary<string, decimal> SpecializationFees = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Cardiologist", 40.00m },
            { "Dermatologist", 30.00m },
            { "Neurologist", 50.00m },
            { "General", 0.00m }
        };

        public static decimal CalculateBillingAmount(string diagnosis, string specialization)
        {
            var basePrice = DiagnosisPrices.ContainsKey(diagnosis)
                ? DiagnosisPrices[diagnosis]
                : DiagnosisPrices["Other"];

            var specializationFee = SpecializationFees.ContainsKey(specialization)
                ? SpecializationFees[specialization]
                : SpecializationFees["General"];

            return basePrice + specializationFee;
        }
        private static readonly Dictionary<(string Diagnosis, string Specialization), decimal> Rates =
            new Dictionary<(string, string), decimal>()
        {
            { ("Flu", "General Practitioner"), 50.00m },
            { ("Checkup", "General Practitioner"), 30.00m },
            { ("X-Ray", "Radiologist"), 100.00m },
            { ("Skin Rash", "Dermatologist"), 70.00m },
            { ("Back Pain", "Orthopedic"), 90.00m },
            // Add more combinations as needed
        };

        public static decimal GetAmount(string diagnosis, string specialization)
        {
            if (Rates.TryGetValue((diagnosis, specialization), out var amount))
            {
                return amount;
            }

            return 60.00m; // Default billing amount
        }
    }
}
