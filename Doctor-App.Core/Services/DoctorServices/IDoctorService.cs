using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Doctor_App.Data.Models;
using Doctor_App.Models.Doctor;
using Doctor_App.Infrastructure.Data.Entities;
using Doctor_App.Core.Models.Doctor;
using Doctor_App.Core.Models.Patient;

namespace Doctor_App.Core.Services.DoctorServices
{
    public interface IDoctorService
    {
        Task<string> GetDoctorIdAsync(string userId);

        Task<bool> ExistsByIdAsync(string userId);

        Task<List<DoctorViewModel>> GetAllDoctorsAsync();

        Task<Guid> AddDoctorAsync(string userId, BecomeDoctorModel model);

        Task<List<PatientViewModel>> GetPatientsByDoctorIdAsync(Guid doctorId);
    }
}
