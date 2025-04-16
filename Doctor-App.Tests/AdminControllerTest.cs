using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Doctor_App.Controllers;
using Doctor_App.Core.Models.Billing;
using Doctor_App.Core.Services.BillingServices;
using Doctor_App.Core.Services.DoctorServices;
using Doctor_App.Core.Services.PatientServices;
using Doctor_App.Core.Services.MedicalRecordServices;
using Doctor_App.Core.Services.AppointmentService;
using Doctor_App.Core.Models.Doctor;

[TestFixture]
public class AdminControllerTests
{
    private Mock<IBillingService> _billingServiceMock;
    private Mock<IDoctorService> _doctorServiceMock;
    private Mock<IPatientService> _patientServiceMock;
    private Mock<IMedicalRecordService> _medicalRecordServiceMock;
    private Mock<IAppointmentService> _appointmentServiceMock;
    private AdminController _controller;

    [SetUp]
    public void Setup()
    {
        _billingServiceMock = new Mock<IBillingService>();
        _doctorServiceMock = new Mock<IDoctorService>();
        _patientServiceMock = new Mock<IPatientService>();
        _medicalRecordServiceMock = new Mock<IMedicalRecordService>();
        _appointmentServiceMock = new Mock<IAppointmentService>();

        _controller = new AdminController(
            _billingServiceMock.Object,
            _doctorServiceMock.Object,
            _patientServiceMock.Object,
            _medicalRecordServiceMock.Object,
            _appointmentServiceMock.Object
        );
    }

    [Test]
    public async Task AllBills_ShouldReturnViewWithBills()
    {
        // Arrange
        var fakeBills = new List<BillViewModel> { new BillViewModel { Id = 1, Amount = 100 } };
        _billingServiceMock.Setup(x => x.GetPendingBillsAsync()).ReturnsAsync(fakeBills);

        // Act
        var result = await _controller.AllBills() as ViewResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<List<BillViewModel>>(result.Model);
    }

    [Test]
    public async Task Review_WithApproveAction_ShouldSetApprovedStatus()
    {
        // Arrange
        var model = new BillViewModel { Id = 1 };
        _billingServiceMock.Setup(x => x.UpdateBillAsync(model)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Review(model, "Approve");

        // Assert
        _billingServiceMock.Verify(x => x.UpdateBillAsync(It.Is<BillViewModel>(b => b.PaymentStatus == "Approved")), Times.Once);
        Assert.IsInstanceOf<RedirectToActionResult>(result);
    }

    [Test]
    public async Task ManageDoctors_ShouldReturnDoctorList()
    {
        _doctorServiceMock.Setup(d => d.GetAllDoctorsAsync())
            .ReturnsAsync(new List<DoctorViewModel>());

        var result = await _controller.ManageDoctors();

        Assert.IsInstanceOf<ViewResult>(result);

        var viewResult = result as ViewResult;
        Assert.IsInstanceOf<List<DoctorViewModel>>(viewResult.Model);
    }
}