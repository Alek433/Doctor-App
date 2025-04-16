using Moq;
using Doctor_App.Controllers;
using Doctor_App.Core.Services.PatientServices;
using Doctor_App.Core.Services.MedicalRecordServices;
using Doctor_App.Core.Services.DoctorServices;
using Doctor_App.Core.Services.BillingServices;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Doctor_App.Core.Models.Patient;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Doctor_App.Infrastructure.Data.Entities;
using System.Security.Claims;
using Doctor_App.Data.Models;
using NUnit.Framework;
using Microsoft.AspNetCore.Http;  // Make sure NUnit is being used

namespace DoctorApp.Tests.Controllers
{
    public class PatientControllerTests
    {
        private readonly Mock<IPatientService> _patientServiceMock;
        private readonly Mock<IMedicalRecordService> _medicalRecordServiceMock;
        private readonly Mock<IDoctorService> _doctorServiceMock;
        private readonly Mock<IBillingService> _billingServiceMock;
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly Mock<DbSet<Patient>> _patientDbSetMock;
        private readonly PatientController _controller;

        public PatientControllerTests()
        {
            _patientServiceMock = new Mock<IPatientService>();
            _medicalRecordServiceMock = new Mock<IMedicalRecordService>();
            _doctorServiceMock = new Mock<IDoctorService>();
            _billingServiceMock = new Mock<IBillingService>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(),
                null, null, null, null, null, null, null, null);

            _patientDbSetMock = new Mock<DbSet<Patient>>(); // Mocking DbSet<Patient>

            // Initialize Controller
            _controller = new PatientController(
                _patientServiceMock.Object,
                _medicalRecordServiceMock.Object,
                _doctorServiceMock.Object,
                _billingServiceMock.Object,
                new DoctorAppDbContext(new DbContextOptions<DoctorAppDbContext>()),
                _userManagerMock.Object);
        }

        [Test]
        public async Task Become_AlreadyPatient_RedirectsToHome()
        {
            // Arrange
            var userId = "testUserId";
            _userManagerMock.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            _patientServiceMock.Setup(s => s.ExistsByIdAsync(userId)).ReturnsAsync(true);

            // Act
            var result = await _controller.Become() as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Home", result.ControllerName);
            Assert.AreEqual("Index", result.ActionName);
        }

        [Test]
        public async Task Become_ValidModel_ReturnsRedirectToHome()
        {
            // Arrange
            var model = new BecomePatientViewModel { /* Set valid model data */ };
            var userId = "testUserId";
            _userManagerMock.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            _patientServiceMock.Setup(s => s.ExistsByIdAsync(userId)).ReturnsAsync(false);
            _patientServiceMock.Setup(s => s.AddPatientAsync(userId, model)).ReturnsAsync(Guid.NewGuid());

            // Act
            var result = await _controller.Become(model) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Home", result.ControllerName);
            Assert.AreEqual("Index", result.ActionName);
        }

        [Test]
        public async Task ViewMyRecords_ValidPatient_ReturnsRecordsView()
        {
            // Arrange
            var userId = "testUserId";
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId)  // Mock the userId claim
    };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));  // Create the ClaimsPrincipal

            // Mock UserManager to return the userId
            _userManagerMock.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);

            // Mock the controller to use the mocked ClaimsPrincipal
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }  // Assign the mock ClaimsPrincipal
            };

            var patient = new Patient { Id = Guid.NewGuid(), UserId = userId };
            _patientServiceMock.Setup(s => s.GetPatientByUserIdAsync(userId)).ReturnsAsync(patient);
            _medicalRecordServiceMock.Setup(s => s.GetPatientRecordsByPatientIdAsync(patient.Id))
                                      .ReturnsAsync(new List<PatientRecordViewModel>());

            // Act
            var result = await _controller.ViewMyRecords() as ViewResult;

            // Assert
            Assert.IsNotNull(result, "The result should not be null.");
            Assert.AreEqual("ViewMyRecords", result.ViewName, "The view name should be 'ViewMyRecords'.");
        }

        [Test]
        public async Task AssignDoctor_ValidPatient_AssignsDoctor()
        {
            // Arrange
            // Arrange
            var userId = "testUserId";
            var doctorId = Guid.NewGuid();
            var patient = new Patient { Id = Guid.NewGuid(), UserId = userId };

            _userManagerMock.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            _patientServiceMock.Setup(s => s.GetPatientByUserIdAsync(userId)).ReturnsAsync(patient);
            _patientServiceMock.Setup(s => s.AssignDoctorToPatientAsync(patient.Id, doctorId)).Returns(Task.CompletedTask);

            // Setup ClaimsPrincipal with the logged-in user
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _controller.AssignDoctor(doctorId);

            // Debugging: Log or inspect the result type
            if (result == null)
            {
                Assert.Fail("The result was null.");
            }

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult, "Expected a redirect result, but got null.");
            Assert.AreEqual("Home", redirectResult.ControllerName);
            Assert.AreEqual("Index", redirectResult.ActionName);
        }
    }
}
