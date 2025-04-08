using HQB.WebApi.Controllers;
using HQB.WebApi.Interfaces;
using HQB.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HQB.WebApi.Tests.Controllers
{
  [TestClass]
  public class DoctorsControllerTest
  {
    public required Mock<ILogger<DoctorsController>> _loggerMock;
    public required Mock<IDoctorRepository> _doctorRepositoryMock;
    public required Mock<IPatientRepository> _patientRepositoryMock;
    public required DoctorsController _controller;

    [TestInitialize]
    public void Setup()
    {
      _loggerMock = new Mock<ILogger<DoctorsController>>();
      _doctorRepositoryMock = new Mock<IDoctorRepository>();
      _patientRepositoryMock = new Mock<IPatientRepository>();
      _controller = new DoctorsController(_loggerMock.Object, _doctorRepositoryMock.Object, _patientRepositoryMock.Object);
    }

    [TestMethod]
    public async Task GetDoctors_ReturnsOkResult_WithListOfDoctors()
    {
      // Arrange
      var doctors = new List<Doctor> { new() { ID = Guid.NewGuid(), UserID = Guid.NewGuid().ToString(), FirstName = "John", LastName = "Smith", Specialization = "Cardiology" } };
      _doctorRepositoryMock.Setup(repo => repo.GetAllDoctorsAsync()).ReturnsAsync(doctors);

      // Act
      var result = await _controller.GetDoctors();

      // Assert
      var okResult = result.Result as OkObjectResult;
      Assert.IsNotNull(okResult);
      Assert.AreEqual(200, okResult.StatusCode);
      Assert.AreEqual(doctors, okResult.Value);
    }

    [TestMethod]
    public async Task GetDoctors_ReturnsNotFound_WhenNoDoctorsExist()
    {
      // Arrange
      _doctorRepositoryMock.Setup(repo => repo.GetAllDoctorsAsync()).ReturnsAsync([]);

      // Act
      var result = await _controller.GetDoctors();

      // Assert
      var notFoundResult = result.Result as NotFoundObjectResult;
      Assert.IsNotNull(notFoundResult);
      Assert.AreEqual(404, notFoundResult.StatusCode);
      Assert.AreEqual("No doctors found.", notFoundResult.Value);
    }

    [TestMethod]
    public async Task GetDoctor_ReturnsOkResult_WithDoctor()
    {
      // Arrange
      var doctorId = Guid.NewGuid();
      var doctor = new Doctor { ID = doctorId, UserID = Guid.NewGuid().ToString(), FirstName = "John", LastName = "Smith", Specialization = "Cardiology" };
      _doctorRepositoryMock.Setup(repo => repo.GetDoctorByIdAsync(doctorId)).ReturnsAsync(doctor);

      // Act
      var result = await _controller.GetDoctor(doctorId);

      // Assert
      var okResult = result.Result as OkObjectResult;
      Assert.IsNotNull(okResult);
      Assert.AreEqual(200, okResult.StatusCode);
      Assert.AreEqual(doctor, okResult.Value);
    }

    [TestMethod]
    public async Task GetDoctor_ReturnsNotFound_WhenDoctorDoesNotExist()
    {
      // Arrange
      var doctorId = Guid.NewGuid();
      _doctorRepositoryMock.Setup(repo => repo.GetDoctorByIdAsync(doctorId)).ReturnsAsync((Doctor?)null);

      // Act
      var result = await _controller.GetDoctor(doctorId);

      // Assert
      var notFoundResult = result.Result as NotFoundObjectResult;
      Assert.IsNotNull(notFoundResult);
      Assert.AreEqual(404, notFoundResult.StatusCode);
      Assert.AreEqual($"Doctor with ID {doctorId} not found.", notFoundResult.Value);
    }

    [TestMethod]
    public async Task AddDoctor_ReturnsCreatedAtActionResult_WithDoctor()
    {
      // Arrange
      var doctor = new Doctor { ID = Guid.NewGuid(), UserID = Guid.NewGuid().ToString(), FirstName = "John", LastName = "Smith", Specialization = "Cardiology" };
      _doctorRepositoryMock.Setup(repo => repo.AddDoctorAsync(doctor)).Returns(Task.FromResult(0));

      // Act
      var result = await _controller.AddDoctor(doctor);

      // Assert
      var createdAtActionResult = result.Result as CreatedAtActionResult;
      Assert.IsNotNull(createdAtActionResult);
      Assert.AreEqual(201, createdAtActionResult.StatusCode);
      Assert.AreEqual(doctor, createdAtActionResult.Value);
    }

    [TestMethod]
    public async Task UpdateDoctor_ReturnsNoContent_WhenDoctorIsUpdated()
    {
      // Arrange
      var doctorId = Guid.NewGuid();
      var doctor = new Doctor { ID = doctorId, UserID = Guid.NewGuid().ToString(), FirstName = "John", LastName = "Smith", Specialization = "Cardiology" };
      _doctorRepositoryMock.Setup(repo => repo.GetDoctorByIdAsync(doctorId)).ReturnsAsync(doctor);
      _doctorRepositoryMock.Setup(repo => repo.UpdateDoctorAsync(doctor)).Returns(Task.FromResult(0));

      // Act
      var result = await _controller.UpdateDoctor(doctorId, doctor);

      // Assert
      var noContentResult = result as NoContentResult;
      Assert.IsNotNull(noContentResult);
      Assert.AreEqual(204, noContentResult.StatusCode);
    }

    [TestMethod]
    public async Task DeleteDoctor_ReturnsNoContent_WhenDoctorIsDeleted()
    {
      // Arrange
      var doctorId = Guid.NewGuid();
      var doctor = new Doctor { ID = doctorId, UserID = Guid.NewGuid().ToString(), FirstName = "John", LastName = "Smith", Specialization = "Cardiology" };
      _doctorRepositoryMock.Setup(repo => repo.GetDoctorByIdAsync(doctorId)).ReturnsAsync(doctor);
      _doctorRepositoryMock.Setup(repo => repo.DeleteDoctorAsync(doctorId)).ReturnsAsync(1);

      // Act
      var result = await _controller.DeleteDoctor(doctorId);

      // Assert
      var noContentResult = result as NoContentResult;
      Assert.IsNotNull(noContentResult);
      Assert.AreEqual(204, noContentResult.StatusCode);
    }
  }
}