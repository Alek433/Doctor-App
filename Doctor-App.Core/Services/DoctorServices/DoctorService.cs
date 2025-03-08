using Doctor_App.Data.Models;
using Doctor_App.Models.Doctor;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Doctor_App.Infrastructure.Data.Entities;
using Doctor_App.Infrastructure.Data.Common;

namespace Doctor_App.Core.Services.DoctorServices
{
    public class DoctorService : IDoctorService
    {
        private readonly IRepository _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DoctorService(IRepository context, UserManager<IdentityUser> userManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<Guid> AddDoctorAsync(string userId, BecomeDoctorModel model)
        {
            if (string.IsNullOrEmpty(userId) || model == null)
            {
                throw new ArgumentNullException(nameof(model), "Invalid doctor data.");
            }

            var doctor = new Doctor()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Specialization = model.Specialization,
                ContactInformation = model.ContactInformation,
                OfficeLocation = model.OfficeLocation
            };
            await _context.AddAsync(doctor);
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, "Doctor"); // Add doctor role
            }
            return doctor.Id;
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

        public async Task<string> GetDoctorIdAsync(string userId)
        {
            var doctor = await this._context.GetByIdAsync<Doctor>(userId);

            if (doctor == null)
            {
                throw new NotImplementedException("Patient not found!");
            }
            return doctor.Id.ToString();
        }

    }
}
