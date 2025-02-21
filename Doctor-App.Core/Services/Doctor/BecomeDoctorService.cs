using Doctor_App.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor_App.Core.Services.Doctor
{
    public class BecomeDoctorService : IBecomeDoctorService
    {
        private readonly DoctorAppDbContext data;
        private readonly UserManager<IdentityUser> _userManager;

        public BecomeDoctorService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> BecomeDoctorAsync(string userId, string contactInformation)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false; // User not found
            }

            // Ensure user is not already a doctor
            if (await _userManager.IsInRoleAsync(user, "Doctor"))
            {
                return false; // Already a doctor
            }

            // Add user to "Doctor" role (remove "Patient" if they are in it)
            if (await _userManager.IsInRoleAsync(user, "Patient"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Patient");
            }

            var result = await _userManager.AddToRoleAsync(user, "Doctor");

            return result.Succeeded;
        }

        public async Task<bool> ExistsByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false; // User not found
            }

            return await _userManager.IsInRoleAsync(user, "Doctor");
        }

        public string GetDoctorId(string userId)
        {
            return this.data.Doctors
                    .First(a => a.Id.ToString() == userId)
                    .Id.ToString();
        }

    }
}
