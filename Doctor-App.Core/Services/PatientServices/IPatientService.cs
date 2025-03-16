using Doctor_App.Core.Models;
using Doctor_App.Core.Models.Patient;
using Doctor_App.Infrastructure.Data.Entities;
using Doctor_App.Models.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor_App.Core.Services.PatientServices
{
    public interface IPatientService
    {
        Task<string> GetPatientIdAsync(string userId);
        Task<bool> ExistsByIdAsync(string userId);
        Task<Guid> AddPatientAsync(string userId, BecomePatientViewModel model);
        Task AssignDoctorToPatientAsync(Guid patientId, Guid doctorId);
        Task<Patient?> GetPatientByUserIdAsync(string userId);
    }
}
