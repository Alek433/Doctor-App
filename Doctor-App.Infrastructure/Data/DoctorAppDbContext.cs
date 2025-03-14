using Microsoft.EntityFrameworkCore;
using Doctor_App.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Doctor_App.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Doctor_App.Infrastructure.Data.Entities;

namespace Doctor_App.Data.Models
{
    public class DoctorAppDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<Billing> Billings { get; set; }
        public DbSet<PatientDoctor> PatientDoctors { get; set; }
        public DoctorAppDbContext(DbContextOptions<DoctorAppDbContext> options)
           : base(options)
        {

        }
        public DoctorAppDbContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            if (!builder.IsConfigured)

                builder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=DoctorApp;Trusted_Connection=True;MultipleActiveResultSets=true");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(x => new { x.LoginProvider, x.ProviderKey });
            modelBuilder.Entity<IdentityUserRole<string>>().HasKey(x => new { x.UserId, x.RoleId });
            modelBuilder.Entity<IdentityUserToken<string>>().HasKey(x => new { x.UserId, x.LoginProvider, x.Name });
            // Configure relationships
            modelBuilder.Entity<PatientDoctor>()
                .HasKey(pd => new { pd.PatientId, pd.DoctorId });
            modelBuilder.Entity<Visit>()
                .HasOne(v => v.Patient)
                .WithMany(p => p.Visits)
                .HasForeignKey(v => v.PatientId);

            modelBuilder.Entity<Visit>()
                .HasOne(v => v.Doctor)
                .WithMany(d => d.Visits)
                .HasForeignKey(v => v.DoctorId);

            modelBuilder.Entity<PatientDoctor>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.PatientDoctors)
                .HasForeignKey(a => a.PatientId);

            modelBuilder.Entity<PatientDoctor>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.PatientDoctors)
                .HasForeignKey(a => a.DoctorId);

            modelBuilder.Entity<Billing>()
                .HasOne(b => b.Visit)
                .WithOne()
                .HasForeignKey<Billing>(b => b.VisitId);

            modelBuilder.Entity<Patient>()
                .HasOne(p => p.User)
                .WithMany() // No inverse navigation from IdentityUser
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
