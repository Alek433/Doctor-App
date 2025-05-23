﻿using Doctor_App.Core.Models.Doctor;
using Doctor_App.Core.Models.Patient;
using Doctor_App.Data.Models;
using Doctor_App.Infrastructure.Data.Common;
using Doctor_App.Infrastructure.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Doctor_App.Core.Services.PatientServices
{
    public class PatientService : IPatientService
    {
        private readonly IRepository _context;
        private readonly DoctorAppDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public PatientService(IRepository context, DoctorAppDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }
        public async Task<Guid> AddPatientAsync(string userId, BecomePatientViewModel model)
        {
            if (string.IsNullOrEmpty(userId) || model == null)
            {
                throw new ArgumentNullException(nameof(model), "Invalid doctor data.");
            }

            var patient = new Patient()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                ContactInformation = model.ContactInformation,
                Address = model.Address,
                EmergencyContact = model.EmergencyContact
            };
            await _context.AddAsync(patient);
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, "Patient"); // Add doctor role
            }
            return patient.Id;
        }

        public async Task<bool> ExistsByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false; // User not found
            }

            return await _userManager.IsInRoleAsync(user, "Patient");
        }
        public async Task<List<PatientViewModel>> GetAllPatientsAsync()
        {
            return await _dbContext.Patients
                .Select(p => new PatientViewModel
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Email = p.User.Email,
                    DateOfBirth= p.DateOfBirth,
                    Gender = p.Gender,
                    ContactInformation= p.ContactInformation
                })
                .ToListAsync();
        }
        public async Task<string> GetPatientIdAsync(string userId)
        {
            var patient = await this._context.GetByIdAsync<Patient>(userId);
            
            if (patient == null)
            {
                throw new NotImplementedException("Patient not found!");
            }
            return patient.Id.ToString();
        }
        public async Task AssignDoctorToPatientAsync(Guid patientId, Guid doctorId)
        {
            var existingAssignment = await _dbContext.PatientDoctors
                   .FirstOrDefaultAsync(pd => pd.PatientId == patientId && pd.DoctorId == doctorId);

            if (existingAssignment == null)
            {
                var patientDoctor = new PatientDoctor
                {
                    PatientId = patientId,
                    DoctorId = doctorId
                };

                _dbContext.PatientDoctors.Add(patientDoctor);
                await _dbContext.SaveChangesAsync();

                Console.WriteLine($"✔️ Patient {patientId} assigned to Doctor {doctorId}.");
            }
            else
            {
                Console.WriteLine($"⚠️ Patient {patientId} is already assigned to Doctor {doctorId}.");
            }
        }
        public async Task<DoctorViewModel> GetAssignedDoctorAsync(string patientId)
        {
            var patient = await _dbContext.Patients
                .Include(p => p.PatientDoctors)
                    .ThenInclude(pd => pd.Doctor)
                .FirstOrDefaultAsync(p => p.UserId == patientId);

            if (patient == null || !patient.PatientDoctors.Any())
                return null;

            var doctor = patient.PatientDoctors.First().Doctor;

            return new DoctorViewModel
            {
                Id = doctor.Id,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                Email = doctor.User?.Email,
                City = doctor.City ?? string.Empty,
                Specialization = doctor.Specialization,
                OfficeLocation = doctor.OfficeLocation,
                ContactInformation = doctor.ContactInformation ?? string.Empty,
                IsApproved = doctor.IsApproved,
                AverageRating = doctor.PatientDoctors.Any(pd => pd.Rating.HasValue)
                     ? doctor.PatientDoctors.Where(pd => pd.Rating.HasValue).Average(pd => pd.Rating.Value)
                     : (double?)null
                // Add more properties if needed
            };
        }
        public async Task<List<Guid>> GetAssignedDoctorIdsAsync(Guid patientId)
        {
            return await _dbContext.PatientDoctors
                .Where(pd => pd.PatientId == patientId)
                .Select(pd => pd.DoctorId)
                .ToListAsync();
        }
        public async Task<bool> IsAlreadyAssigned(string patientId, string doctorId)
        {
            return await _dbContext.PatientDoctors.AnyAsync(pd => pd.PatientId.ToString() == patientId && pd.DoctorId.ToString() == doctorId);
        }
        public async Task<Patient?> GetPatientByUserIdAsync(string userId)
        {
            return await _dbContext.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
        }
    }
}
