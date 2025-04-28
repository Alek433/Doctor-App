// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Doctor_App.Core.Models.Doctor;
using Doctor_App.Core.Models.Patient;
using Doctor_App.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Doctor_App.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly DoctorAppDbContext _context;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            DoctorAppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        [BindProperty]
        public PatientViewModel PatientInfo { get; set; }

        [BindProperty]
        public DoctorViewModel DoctorInfo { get; set; }
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            Username = user.UserName;

            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            if (role == "Patient")
            {
                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.UserId == user.Id);

                if (patient != null)
                {
                    PatientInfo = new PatientViewModel
                    {
                        FirstName = patient.FirstName,
                        LastName = patient.LastName,
                        DateOfBirth = patient.DateOfBirth,
                        Gender = patient.Gender,
                        ContactInformation = patient.ContactInformation,
                        // Fill other fields if you have them
                    };
                }
            }
            else if (role == "Doctor")
            {
                var doctor = await _context.Doctors
                    .FirstOrDefaultAsync(d => d.UserId == user.Id);

                if (doctor != null)
                {
                    DoctorInfo = new DoctorViewModel
                    {
                        FirstName = doctor.FirstName,
                        LastName = doctor.LastName,
                        Specialization = doctor.Specialization,
                        City = doctor.City,
                        OfficeLocation = doctor.OfficeLocation,
                        ContactInformation = doctor.ContactInformation,
                        // Fill other fields if you have them
                    };
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            if (role == "Patient")
            {
                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.UserId == user.Id);

                if (patient != null)
                {
                    patient.FirstName = PatientInfo.FirstName;
                    patient.LastName = PatientInfo.LastName;
                    patient.DateOfBirth = PatientInfo.DateOfBirth;
                    patient.Gender = PatientInfo.Gender;
                    patient.ContactInformation = PatientInfo.ContactInformation;
                    // Update any other fields you added
                }
            }
            else if (role == "Doctor")
            {
                var doctor = await _context.Doctors
                    .FirstOrDefaultAsync(d => d.UserId == user.Id);

                if (doctor != null)
                {
                    doctor.FirstName = DoctorInfo.FirstName;
                    doctor.LastName = DoctorInfo.LastName;
                    doctor.Specialization = DoctorInfo.Specialization;
                    doctor.City = DoctorInfo.City;
                    doctor.OfficeLocation = DoctorInfo.OfficeLocation;
                    doctor.ContactInformation = DoctorInfo.ContactInformation;
                    // Update any other fields you added
                }
            }

            await _context.SaveChangesAsync();

            StatusMessage = "Your profile has been updated!";
            return RedirectToPage();
        }
    }
}
