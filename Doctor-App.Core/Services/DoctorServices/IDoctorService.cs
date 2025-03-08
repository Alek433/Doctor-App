using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Doctor_App.Data.Models;
using Doctor_App.Models.Doctor;
using Doctor_App.Infrastructure.Data.Entities;

namespace Doctor_App.Core.Services.DoctorServices
{
    public interface IDoctorService
    {
        Task<string> GetDoctorIdAsync(string userId);

        Task<bool> ExistsByIdAsync(string userId);

        Task<Guid> AddDoctorAsync(string userId, BecomeDoctorModel model);
    }
}
