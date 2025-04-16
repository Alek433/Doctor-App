using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Doctor_App.Core.Models.Appointment;
using Doctor_App.Infrastructure.Data;
using Doctor_App.Core.Services;
using System.Collections.Generic;
using Doctor_App.Core.Services.AppointmentService;
using Doctor_App.Core.Models.Doctor;
using Doctor_App.Data.Models;
using Doctor_App.Core.Services.DoctorServices;
using Doctor_App.Models.Doctor;
using Microsoft.AspNetCore.Identity;
using Doctor_App.Infrastructure.Data.Common;
using Doctor_App.Infrastructure.Data.Entities;

[TestFixture]
public class AppointmentServiceTests
{
    private AppointmentService _service;
    private DoctorService _doctorService;
    private DoctorAppDbContext _dbContext;
    private Mock<UserManager<IdentityUser>> _userManagerMock;
    private Mock<IRepository> _repoMock;

    [SetUp]
    public void SetUp()
    {
        // Use InMemoryDatabase for testing
        var options = new DbContextOptionsBuilder<DoctorAppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new DoctorAppDbContext(options);
        _repoMock = new Mock<IRepository>();

        var store = new Mock<IUserStore<IdentityUser>>();
        _userManagerMock = new Mock<UserManager<IdentityUser>>(
            store.Object, null, null, null, null, null, null, null, null
        );
        _doctorService = new DoctorService(_repoMock.Object, _dbContext, _userManagerMock.Object);
        _service = new AppointmentService(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Test]
    public async Task CreateAppointmentAsync_WithValidModel_ReturnsTrueAndAddsAppointment()
    {
        Guid patientId = Guid.Parse("9cfe5f9f-fd10-4982-babf-666049238a09");
        Guid doctorId = Guid.Parse("00429510-92bc-4cd7-a8dc-5c81ad88b3e0");

        var patient = new Patient
        {
            Id = patientId,
            FirstName = "Test",
            LastName = "Patient",
            Address = "123 Main St",
            ContactInformation = "patient@example.com",
            EmergencyContact = "911",
            Gender = "Other",
            UserId = "patient-user-id"
        };
        var doctor = new Doctor
        {
            Id = doctorId,
            FirstName = "Test",
            LastName = "Doctor",
            Specialization = "General",
            ContactInformation= "2378485",
            OfficeLocation = "123 Saint St",
            UserId = "some-user-id"
        };

        await _dbContext.Patients.AddAsync(patient);
        await _dbContext.Doctors.AddAsync(doctor);
        await _dbContext.SaveChangesAsync();
        // Arrange
        var model = new AppointmentViewModel
        {
            PatientId = patientId,
            DoctorId = doctorId,
            AppointmentDate = DateTime.Today.AddDays(1),
            Reason = "Test Reason"
        };

        // Act
        var result = await _service.CreateAppointmentAsync(model);
        var appointment = await _dbContext.Appointments.FirstOrDefaultAsync();

        // Assert
        Assert.IsTrue(result);
        Assert.IsNotNull(appointment);
        Assert.AreEqual("Test Reason", appointment.Reason);
        Assert.AreEqual("Scheduled", appointment.Status);
    }

    [Test]
    public async Task CreateAppointmentAsync_WithNullModel_ReturnsFalse()
    {
        // Act
        var result = await _service.CreateAppointmentAsync(null);

        // Assert
        Assert.IsFalse(result);
    }
    [Test]
    public async Task AddDoctorAsync_ValidData_ShouldCreateDoctor()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var model = new BecomeDoctorModel
        {
            FirstName = "Alice",
            LastName = "Smith",
            Specialization = "Dermatology",
            ContactInformation = "alice@example.com",
            OfficeLocation = "Room 22"
        };

        var mockUser = new IdentityUser { Id = userId };
        _userManagerMock.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(mockUser);
        _userManagerMock.Setup(u => u.AddToRoleAsync(mockUser, "Doctor")).ReturnsAsync(IdentityResult.Success);

        Doctor addedDoctor = null;
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Doctor>()))
            .Callback<Doctor>(d => addedDoctor = d)
            .Returns(Task.CompletedTask);

        _repoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _doctorService.AddDoctorAsync(userId, model);

        // Assert
        Assert.IsNotNull(addedDoctor);
        Assert.AreEqual("Alice", addedDoctor.FirstName);
        Assert.AreEqual("Smith", addedDoctor.LastName);
        Assert.AreEqual(result, addedDoctor.Id);

        _userManagerMock.Verify(u => u.AddToRoleAsync(mockUser, "Doctor"), Times.Once);
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Doctor>()), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}