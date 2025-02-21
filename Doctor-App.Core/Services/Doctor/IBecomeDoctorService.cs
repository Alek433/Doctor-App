using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor_App.Core.Services.Doctor
{
    public interface IBecomeDoctorService
    {
        string GetDoctorId(string userId);

        Task<bool> ExistsByIdAsync(string userId);

        Task<bool> BecomeDoctorAsync(string userId, string contactInformation);
    }
}
