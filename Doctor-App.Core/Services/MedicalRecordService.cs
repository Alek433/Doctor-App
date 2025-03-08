using Doctor_App.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Doctor_App.Infrastructure.Data.Entities;
using Doctor_App.Core.Models;

namespace Doctor_App.Core.Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly DoctorAppDbContext _context;

        public MedicalRecordService(DoctorAppDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddPatientRecordAsync(PatientRecordViewModel model)
        {
            var visit = new Visit()
            {
                Id = model.Id,
                PatientId = model.PatientId,
                DoctorId = model.DoctorId,
                VisitDate = model.VisitDate,
                ReasonForVisit = model.ReasonForVisit,
                Diagnosis = model.Diagnosis,
                Prescriptions = model.Prescriptions,
                Notes = model.Notes,
            };
            _context.Visits.Add(visit);
            await _context.SaveChangesAsync();
            return visit.Id;
        }

        public async Task<bool> DeletePatientRecordAsync(Guid id)
        {
            var visit = await _context.Visits.FindAsync(id);
            if (visit == null)
            {
                return false;
            }
            _context.Visits.Remove(visit);
            await _context.SaveChangesAsync();
            return true;
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
        public async Task<Visit?> GetPatientRecordAsync(int id)
        {
            return await _context.Visits.FirstOrDefaultAsync(r => r.Id == id);
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
                .Where(v => v.DoctorId.ToString() == doctorId)
                .Include(v => v.Patient) // Assuming you have a navigation property for the patient
                .Select(v => new PatientRecordViewModel
                {
                    Id = v.Id,
                    PatientId = v.PatientId,
                    DoctorId = v.DoctorId,
                    VisitDate = v.VisitDate,
                    ReasonForVisit = v.ReasonForVisit,
                    Diagnosis = v.Diagnosis,
                    Prescriptions = v.Prescriptions,
                    Notes = v.Notes
                })
                .ToListAsync();
        }

        public async Task<bool> UpdatePatientRecordAsync(Guid id, PatientRecordViewModel model)
        {
            var visit = await _context.Visits.FindAsync(id);

            if (visit == null)
            {
                return false;
            }
            visit.VisitDate = model.VisitDate;
            visit.ReasonForVisit = model.ReasonForVisit;
            visit.Diagnosis = model.Diagnosis;
            visit.Prescriptions = model.Prescriptions;
            visit.Notes = model.Notes;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
