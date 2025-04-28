using Doctor_App.Core.Models.Doctor;
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
        Task<List<PatientViewModel>> GetAllPatientsAsync();
        Task AssignDoctorToPatientAsync(Guid patientId, Guid doctorId);
        Task<DoctorViewModel> GetAssignedDoctorAsync(string patientId);
        Task<List<Guid>> GetAssignedDoctorIdsAsync(Guid patientId);
        Task<bool> IsAlreadyAssigned(string patientId, string doctorId);
        Task<Patient?> GetPatientByUserIdAsync(string userId);
    }
}
