using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Doctor_App.Core.Models;
using Doctor_App.Data.Models;
using Doctor_App.Infrastructure.Data.Entities;

namespace Doctor_App.Core.Services
{
    public interface IMedicalRecordService
    {
        Task<int> AddPatientRecordAsync(PatientRecordViewModel model);
        Task<Visit?> GetPatientRecordAsync(int id);
        Task<List<PatientRecordViewModel>> GetPatientRecordsByDoctorIdAsync(string doctorId);
        Task<IEnumerable<PatientRecordViewModel>> GetAllPatientRecordsAsync();
        Task<bool> UpdatePatientRecordAsync(Guid id, PatientRecordViewModel model);
        Task<bool> DeletePatientRecordAsync(Guid id);
    }
}
