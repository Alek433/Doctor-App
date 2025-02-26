using Doctor_App.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Doctor_App.Infrastructure.Data.Entities;

namespace Doctor_App.Core.Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly DoctorAppDbContext _context;

        public MedicalRecordService(DoctorAppDbContext context)
        {
            _context = context;
        }

        public async Task AddRecordAsync(Visit record)
        {
            _context.Visits.Add(record);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Visit>> GetRecordsForPatientAsync(Guid patientId)
        {
            return await _context.Visits
                .Where(r => r.PatientId == patientId)
                .ToListAsync();
        }
    }
}
