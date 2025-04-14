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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using Doctor_App.Core.Models.Doctor;
using Doctor_App.Core.Models.Patient;

namespace Doctor_App.Core.Services.DoctorServices
{
    public class DoctorService : IDoctorService
    {
        private readonly IRepository _context;
        private readonly DoctorAppDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public DoctorService(IRepository context, DoctorAppDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
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
        public async Task<List<DoctorViewModel>> GetAllDoctorsAsync()
        {
            return await _dbContext.Doctors
                .Select(d => new DoctorViewModel
                {
                    Id = d.Id,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    Email = d.User.Email,
                    Specialization= d.Specialization,
                })
                .ToListAsync();
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
        public async Task<List<PatientViewModel>> GetPatientsByDoctorIdAsync(Guid doctorId)
        {
            var patients = await _dbContext.PatientDoctors
                     .Where(pd => pd.DoctorId == doctorId)
                     .Include(pd => pd.Patient)
                     .ToListAsync();
            Console.WriteLine($"Doctor {doctorId} has {patients.Count} patients.");
            // Convert to ViewModel
            return patients
                     .Where(p => p.Patient != null) // Ensure only valid patients are returned
                     .Select(p => new PatientViewModel
                     {
                         Id = p.Patient.Id,
                         FirstName = p.Patient.FirstName,
                         LastName = p.Patient.LastName,
                         DateOfBirth = p.Patient.DateOfBirth,
                         Gender = p.Patient.Gender,
                         ContactInformation = p.Patient.ContactInformation
                     }).ToList();
        }
    }
}
