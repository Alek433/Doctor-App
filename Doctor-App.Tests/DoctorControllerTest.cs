using Doctor_App.Controllers;
using Doctor_App.Core.Models.Patient;
using Doctor_App.Core.Models.Visit;
using Doctor_App.Core.Services.DoctorServices;
using Doctor_App.Core.Services.MedicalRecordServices;
using Doctor_App.Data.Models;
using Doctor_App.Infrastructure.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Doctor_App.Core.Services.BillingServices;

namespace Doctor_App.Tests
{
    public class DoctorControllerTest
    {
        private Mock<IDoctorService> _doctorServiceMock;
        private Mock<IMedicalRecordService> _medicalRecordServiceMock;
        private Mock<IBillingService> _billingServiceMock;
        private Mock<UserManager<IdentityUser>> _userManagerMock;
        private DoctorController _controller;
        private DoctorAppDbContext _dbContext;

        [SetUp]
        public void Setup()
        {
            _doctorServiceMock = new Mock<IDoctorService>();
            _medicalRecordServiceMock = new Mock<IMedicalRecordService>();
            _billingServiceMock = new Mock<IBillingService>();
            _userManagerMock = MockUserManager();

            var store = new Mock<IUserStore<IdentityUser>>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>(
                store.Object, null, null, null, null, null, null, null, null
            );
            // Setup In-Memory DbContext
            var options = new DbContextOptionsBuilder<DoctorAppDbContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            var _dbContext = new DoctorAppDbContext(options);

            // Add mock data to the In-Memory DbContext
            _dbContext.Doctors.Add(new Doctor
            {
                Id = Guid.NewGuid(),
                UserId = "test-user-id",
                ContactInformation = "123-456-7890", // Add missing properties
                FirstName = "John",                 // Add missing properties
                LastName = "Doe",                   // Add missing properties
                OfficeLocation = "Room 101",        // Add missing properties
                Specialization = "Cardiology",      // Add missing properties
                City = "New York"                   // Add missing properties
            });
            _dbContext.SaveChanges();


            // Instantiate the controller with the in-memory DbContext and services
            _controller = new DoctorController(
                _doctorServiceMock.Object,
                _medicalRecordServiceMock.Object,
                _billingServiceMock.Object,
                _dbContext,
                _userManagerMock.Object
            );

            // Set the user context (mocking the user claims)
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        private Mock<UserManager<IdentityUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            return new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
        }

        [Test]
        public async Task Become_Get_WhenAlreadyDoctor_ShouldRedirect()
        {
            _userManagerMock.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns("test-user-id");

            _doctorServiceMock.Setup(d => d.ExistsByIdAsync("test-user-id"))
                .ReturnsAsync(true);

            var result = await _controller.Become();

            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public async Task MyPatients_WhenDoctorExists_ShouldReturnViewWithPatients()
        {
            // Set up a valid doctor instance with a GUID and user ID
            var doctorId = Guid.NewGuid();
            var doctor = new Doctor
            {
                Id = doctorId,
                UserId = "test-user-id",
                ContactInformation = "123-456-7890",  // Ensure required property is populated
                FirstName = "John",                   // Ensure required property is populated
                LastName = "Doe",                     // Ensure required property is populated
                OfficeLocation = "Room 101",          // Ensure required property is populated
                Specialization = "Cardiology",
                City = "New York"                     // Ensure required property is populated
            };

            // Set up the in-memory DbContext for testing
            var options = new DbContextOptionsBuilder<DoctorAppDbContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            // Create a new in-memory database context
            using (var dbContext = new DoctorAppDbContext(options))
            {
                // Add a doctor record to the in-memory database
                dbContext.Doctors.Add(doctor);
                dbContext.SaveChanges();

                // Set up the doctor service mock
                _doctorServiceMock.Setup(s => s.GetPatientsByDoctorIdAsync(doctor.Id))
                    .ReturnsAsync(new List<PatientViewModel>());

                // Set up the controller with the in-memory DbContext
                _controller = new DoctorController(
                    _doctorServiceMock.Object,
                    _medicalRecordServiceMock.Object,
                    _billingServiceMock.Object,
                    dbContext, // Use the in-memory DbContext
                    _userManagerMock.Object
                );

                // Set the user context (mocking the user claims)
                var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id")
                }, "mock"));

                _controller.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                };

                // Call the action
                var result = await _controller.MyPatients();

                // Assert that the result is a ViewResult
                Assert.IsInstanceOf<ViewResult>(result);
            }
        }

        [Test]
        public async Task Dashboard_ShouldReturnDashboardViewModel()
        {
            _medicalRecordServiceMock.Setup(s => s.GetPatientRecordsByDoctorIdAsync("test-user-id"))
                .ReturnsAsync(new List<PatientRecordViewModel>());

            _medicalRecordServiceMock
                .Setup(s => s.GetVisitStatsAsync("test-user-id"))
                .ReturnsAsync(new List<VisitStatsViewModel>()); // <-- make sure this is not a List

            _medicalRecordServiceMock.Setup(s => s.GetAllVisitsAsync())
                .ReturnsAsync(new List<VisitViewModel>());

            var result = await _controller.Dashboard();

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task Add_Get_ShouldReturnAddView()
        {
            // Setup a valid GUID for the doctor and patient
            var doctorId = Guid.NewGuid();  // Use valid GUID format
            var patientList = new List<SelectListItem>
    {
        new SelectListItem { Value = Guid.NewGuid().ToString(), Text = "Patient 1" },
        new SelectListItem { Value = Guid.NewGuid().ToString(), Text = "Patient 2" }
    };

            // Setup mock for GetPatientsForDoctorAsync
            _medicalRecordServiceMock.Setup(m => m.GetPatientsForDoctorAsync(doctorId))
                .ReturnsAsync(patientList);

            // Pass valid GUID for the user (doctor) in the Controller context
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, doctorId.ToString()) // Ensure the GUID is valid
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Execute the action
            var result = await _controller.Add();

            // Assert that the result is a ViewResult
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task Add_Post_ShouldRedirectToDashboard()
        {
            var model = new PatientRecordViewModel();
            _medicalRecordServiceMock.Setup(s => s.AddPatientRecordAsync(model, "test-user-id"))
                .ReturnsAsync(1);

            var result = await _controller.Add(model);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public async Task Edit_Get_ShouldReturnEditView_WhenRecordExists()
        {
            var record = new PatientRecordViewModel { Id = 1 };
            _medicalRecordServiceMock.Setup(s => s.GetPatientRecordByIdAsync(1)).ReturnsAsync(record);

            var result = await _controller.Edit(1);

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task Edit_Post_ValidModel_ShouldRedirectToDashboard()
        {
            var model = new PatientRecordViewModel { Id = 1 };

            var result = await _controller.Edit(model);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public async Task Delete_ShouldRedirectToDashboard_WhenRecordExists()
        {
            _medicalRecordServiceMock.Setup(s => s.GetPatientRecordByIdAsync(1))
                .ReturnsAsync(new PatientRecordViewModel());

            var result = await _controller.Delete(1);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }

        [Test]
        public async Task VisitsByDate_ShouldReturnPartialView()
        {
            var date = DateTime.Today;
            _medicalRecordServiceMock.Setup(s => s.GetVisitsByDateAsync(date))
                .ReturnsAsync(new List<VisitViewModel>());

            var result = await _controller.VisitsByDate(date);

            Assert.IsInstanceOf<PartialViewResult>(result);
        }
    }
}
