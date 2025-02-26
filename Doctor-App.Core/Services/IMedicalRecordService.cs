using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Doctor_App.Data.Models;
using Doctor_App.Infrastructure.Data.Entities;

namespace Doctor_App.Core.Services
{
    public interface IMedicalRecordService
    {
        Task AddRecordAsync(Visit record);
        Task<IEnumerable<Visit>> GetRecordsForPatientAsync(Guid patientId);
    }
}
