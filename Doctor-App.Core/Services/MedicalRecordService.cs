﻿using Doctor_App.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Doctor_App.Infrastructure.Data.Entities;
using Doctor_App.Core.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Doctor_App.Infrastructure.Data.Common;

namespace Doctor_App.Core.Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly DoctorAppDbContext _context;
        private readonly IRepository _repository;
        public MedicalRecordService(DoctorAppDbContext context, IRepository repository)
        {
            _context = context;
            _repository = repository;
        }

        public async Task<int> AddPatientRecordAsync(PatientRecordViewModel model, string doctorUserId)
        {
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == doctorUserId);
            if (doctor == null)
            {
                throw new Exception("Doctor not found.");
            }

            var patient = await _context.Patients.FindAsync(model.PatientId);
            if (patient == null)
            {
                throw new Exception("Invalid patient selection.");
            }

            var visit = new Visit()
            {
                PatientId = model.PatientId,
                DoctorId = doctor.Id, // Ensure correct DoctorId is assigned
                VisitDate = model.VisitDate,
                ReasonForVisit = model.ReasonForVisit,
                Diagnosis = model.Diagnosis,
                Prescriptions = model.Prescriptions,
                Notes = model.Notes,
            };

            _context.Visits.Add(visit);
            await _context.SaveChangesAsync();
            Console.WriteLine($"New visit added: ID={visit.Id}, DoctorId={visit.DoctorId}, PatientId={visit.PatientId}");
            return visit.Id;
        }

        public async Task DeletePatientRecordAsync(int id)
        {
            var visit = await _context.Visits.FindAsync(id);
            if (visit != null)
            {
                _context.Visits.Remove(visit);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<SelectListItem>> GetPatientsForDoctorAsync(Guid doctorId)
        {
            return await _context.PatientDoctors
                .Where(pd => pd.DoctorId == doctorId)
                .Select(pd => new SelectListItem
                {
                    Value = pd.PatientId.ToString(),
                    Text = pd.Patient.FirstName + " " + pd.Patient.LastName
                })
                .ToListAsync();
        }
        public async Task<IEnumerable<PatientRecordViewModel>> GetAllPatientRecordsAsync()
        {
            return await _context.Visits.Select(x => new PatientRecordViewModel
            {
                Id = x.Id,
                PatientId = x.PatientId,
                DoctorId = x.DoctorId,
                VisitDate = x.VisitDate,
                ReasonForVisit = x.ReasonForVisit,
                Diagnosis = x.Diagnosis,
                Prescriptions = x.Prescriptions,
                Notes = x.Notes,
            })
                .ToListAsync();
        }
        public async Task<Visit?> GetPatientRecordByDoctorAsync(Guid doctorUserId)
        {
            return await _context.Visits
                .Include(r => r.Doctor)
                .FirstOrDefaultAsync(r => r.Doctor.UserId == doctorUserId.ToString());
        }
        public async Task<List<PatientRecordViewModel>> GetPatientRecordsAsync(string userId)
        {
            var patient = await _context.Patients
                .Include(p => p.Visits) // Ensure records are loaded
                .FirstOrDefaultAsync(p => p.Id.ToString() == userId);

            if (patient == null)
            {
                throw new Exception("Patient not found.");
            }

            return patient.Visits
                .Select(record => new PatientRecordViewModel
                {
                    Id = record.Id,
                    DoctorId = record.DoctorId,
                    Diagnosis = record.Diagnosis,
                    VisitDate = record.VisitDate,
                    ReasonForVisit = record.ReasonForVisit,
                    Prescriptions = record.Prescriptions,
                    Notes = record.Notes
                }).ToList();
        }
        /*public async Task<List<Patient>> GetPatientsByDoctorIdAsync(Guid doctorId)
        {
            return await _context.Appointments
                .Where(dp => dp.DoctorId == doctorId)
                .Select(dp => dp.Patient)
                .ToListAsync();
        }*/
        public async Task<List<PatientRecordViewModel>> GetPatientRecordsByDoctorIdAsync(string doctorId)
        {
            return await _context.Visits
                .Where(v => v.Doctor.UserId == doctorId)
                .Include(v => v.Doctor)
                .Include(v => v.Patient)
                .OrderByDescending(v => v.VisitDate)
                .Select(v => new PatientRecordViewModel
                {
                    Id = v.Id,
                    PatientId = v.PatientId,
                    DoctorId = v.DoctorId,
                    VisitDate = v.VisitDate,
                    ReasonForVisit = v.ReasonForVisit,
                    Diagnosis = v.Diagnosis,
                    Prescriptions = v.Prescriptions,
                    Notes = v.Notes,
                    FirstName = v.Patient.FirstName, 
                    LastName = v.Patient.LastName
                })
                .ToListAsync();
        }

        public async Task<bool> UpdatePatientRecordAsync(PatientRecordViewModel model)
        {
            var visit = await _context.Visits.FindAsync(model.Id);

            if (visit == null)
            {
                return false;
            }
            visit.VisitDate = model.VisitDate;
            visit.ReasonForVisit = model.ReasonForVisit;
            visit.Diagnosis = model.Diagnosis;
            visit.Prescriptions = model.Prescriptions;
            visit.Notes = model.Notes;

            _context.Visits.Update(visit);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<SelectListItem>> GetPatientsForDoctorAsync(string doctorUserId)
        {
            var doctor = await _context.Doctors
                .Include(d => d.PatientDoctors)
                .ThenInclude(pd => pd.Patient)
                .FirstOrDefaultAsync(d => d.UserId == doctorUserId);

            if (doctor == null)
            {
                throw new Exception("Doctor not found.");
            }

            return doctor.PatientDoctors
                .Select(pd => new SelectListItem
                {
                    Value = pd.Patient.Id.ToString(),
                    Text = pd.Patient.FirstName + " " + pd.Patient.LastName
                })
                .ToList();
        }

        public async Task<List<PatientRecordViewModel>> GetPatientRecordsByPatientIdAsync(Guid patientId)
        {
            return await _context.Visits
                  .Where(v => v.PatientId == patientId)
                  .OrderByDescending(v => v.VisitDate)
                  .Select(v => new PatientRecordViewModel
                  {
                       Id = v.Id,
                       DoctorId = v.DoctorId,
                       PatientId = v.PatientId,
                       FirstName = v.Patient.FirstName,
                       LastName = v.Patient.LastName,
                       VisitDate = v.VisitDate,
                       ReasonForVisit = v.ReasonForVisit,
                       Diagnosis = v.Diagnosis,
                       Prescriptions = v.Prescriptions,
                       Notes = v.Notes
                 })
                 .ToListAsync();
        }
        public async Task<PatientRecordViewModel?> GetPatientRecordByIdAsync(int id)
        {
            return await _context.Visits
                .Where(v => v.Id == id)
                .Include(v => v.Patient)
                .Select(v => new PatientRecordViewModel
                {
                    Id = v.Id,
                    PatientId = v.PatientId,
                    DoctorId = v.DoctorId,
                    VisitDate = v.VisitDate,
                    ReasonForVisit = v.ReasonForVisit,
                    Diagnosis = v.Diagnosis,
                    Prescriptions = v.Prescriptions,
                    Notes = v.Notes,
                    FirstName = v.Patient.FirstName,
                    LastName = v.Patient.LastName
                })
                .FirstOrDefaultAsync(); 
        }
    }
}
