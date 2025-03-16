using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Doctor_App.Core.Models;
using Doctor_App.Data.Models;
using Doctor_App.Infrastructure.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Doctor_App.Core.Services
{
    public interface IMedicalRecordService
    {
        Task<int> AddPatientRecordAsync(PatientRecordViewModel model, string doctorUserId);
        Task<List<SelectListItem>> GetPatientsForDoctorAsync(string doctorUserId);
        Task<List<SelectListItem>> GetPatientsForDoctorAsync(Guid doctorId);
        Task<PatientRecordViewModel?> GetPatientRecordByIdAsync(int id);
        Task<List<PatientRecordViewModel>> GetPatientRecordsByPatientIdAsync(Guid patientId);
        Task<List<PatientRecordViewModel>> GetPatientRecordsByDoctorIdAsync(string doctorId);
        Task<IEnumerable<PatientRecordViewModel>> GetAllPatientRecordsAsync();
        Task<bool> UpdatePatientRecordAsync(PatientRecordViewModel model);
        Task DeletePatientRecordAsync(int id);
    }
}
