using Doctor_App.Data.Models;
using Doctor_App.Infrastructure.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Doctor_App.Infrastructure
{
    public static class SeedData
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var dbContext = serviceProvider.GetRequiredService<DoctorAppDbContext>();

            // Roles to create
            string[] roles = { "Admin", "Doctor", "Patient" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Admin user details
            var adminEmail = "admin@hospital.com";
            var adminPassword = "Admin@123"; // Change to something more secure

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var user = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
            var userEmail = "user@hospital.com";
            var userPassword = "User@123"; // Change to something more secure

            var normalUser = await userManager.FindByEmailAsync(userEmail);

            if (normalUser == null)
            {
                var user = new IdentityUser
                {
                    UserName = userEmail,
                    Email = userEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, userPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");
                }
            }
            // Seed Doctor User
            var doctorEmail = "doctor@hospital.com";
            var doctorPassword = "Doctor@123";

            var doctorUser = await userManager.FindByEmailAsync(doctorEmail);
            if (doctorUser == null)
            {
                doctorUser = new IdentityUser
                {
                    UserName = doctorEmail,
                    Email = doctorEmail,
                    EmailConfirmed = true
                };

                if ((await userManager.CreateAsync(doctorUser, doctorPassword)).Succeeded)
                {
                    await userManager.AddToRoleAsync(doctorUser, "Doctor");

                    // Create corresponding Doctor entity
                    if (!dbContext.Doctors.Any(d => d.UserId == doctorUser.Id))
                    {
                        dbContext.Doctors.Add(new Doctor
                        {
                            UserId = doctorUser.Id,
                            FirstName = "John",
                            LastName = "Smith",
                            Specialization = "Cardiology",
                            ContactInformation = "1234567890",
                            OfficeLocation = "Room 101"
                        });
                    }
                }
            }

            // Seed Patient User
            var patientEmail = "patient@hospital.com";
            var patientPassword = "Patient@123";

            var patientUser = await userManager.FindByEmailAsync(patientEmail);
            if (patientUser == null)
            {
                patientUser = new IdentityUser
                {
                    UserName = patientEmail,
                    Email = patientEmail,
                    EmailConfirmed = true
                };

                if ((await userManager.CreateAsync(patientUser, patientPassword)).Succeeded)
                {
                    await userManager.AddToRoleAsync(patientUser, "Patient");

                    // Create corresponding Patient entity
                    if (!dbContext.Patients.Any(p => p.UserId == patientUser.Id))
                    {
                        dbContext.Patients.Add(new Patient
                        {
                            UserId = patientUser.Id,
                            FirstName = "Jane",
                            LastName = "Doe",
                            Gender = "Female",
                            Address = "456 Elm Street",
                            ContactInformation = "9876543210",
                            EmergencyContact = "Emergency Contact Name"
                        });
                    }
                }
            }

            await dbContext.SaveChangesAsync();
        }
    }
}