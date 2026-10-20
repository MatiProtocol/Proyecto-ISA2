using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PharmaGo.DataAccess;
using PharmaGo.DataAccess.Repositories;
using PharmaGo.Domain.Entities;
using PharmaGo.Domain.Enums;
using PharmaGo.IBusinessLogic;
using System;
using System.Linq;

namespace PharmaGo.Test.DataAccess.Test;

[TestClass]
public class BaseRepositoryTests
{
    private Mock<PharmacyGoDbContext> _mockContext = null!;
    private Mock<ILogManager> _logManagerMock = null!;
    private TestRepository _repository = null!;
    private Booking _booking = null!;

    private class TestRepository : BaseRepository<Booking>
    {
        public TestRepository(PharmacyGoDbContext context, ILogManager logManager) : base(context, logManager) { }
    }

    [TestInitialize]
    public void Initialize()
    {
        var options = new DbContextOptionsBuilder<PharmacyGoDbContext>().UseInMemoryDatabase(databaseName: "testDB").Options;

        _mockContext = new Mock<PharmacyGoDbContext>(options);
        _logManagerMock = new Mock<ILogManager>();
        _repository = new TestRepository(_mockContext.Object, _logManagerMock.Object);

        BookingDetail bookingDetail = new BookingDetail { Code = "PERIFAR", Name = "Perifar", PharmacyName = "Pharmacy Ala" };
        _booking = new Booking
        {
            Id = 1,
            BookerEmail = "test@test.com",
            BookingDate = DateTime.Now.ToString("dd/MM/yyyy"),
            PublicKey = "public key",
            Details = new List<BookingDetail> { bookingDetail }
        };
    }

    [TestMethod]
    public void Save_WhenSaveChangesThrows_EmitsCriticalLog()
    {
        _mockContext.Setup(c => c.SaveChanges()).Throws(new Exception("DB save failure"));

        var ex = Assert.ThrowsException<Exception>(() => _repository.Save());
        Assert.IsTrue(ex.Message.Contains("DB save failure"));

        _logManagerMock.Verify(m => m.Create(
            LogLevel.Critical,
            "Failed to persist changes to database",
            It.Is<string>(s => s.Contains("DB save failure") || s.Contains(nameof(Booking)))),
            Times.Once);
    }

    [TestMethod]
    public void GetAllByExpression_WhenDbSetThrows_EmitsErrorLog()
    {
        _mockContext.Setup(c => c.Set<Booking>()).Throws(new Exception("DB read failure"));

        var ex = Assert.ThrowsException<Exception>(() => _repository.GetAllByExpression(b => true).ToList());
        Assert.IsTrue(ex.Message.Contains("DB read failure"));

        _logManagerMock.Verify(m => m.Create(
                LogLevel.Error,
                "Failed to read from repository",
                It.Is<string>(s => s.Contains("DB read failure") || s.Contains(nameof(Booking)))),
            Times.Once);
    }

    [TestMethod]
    public void GetOneByExpression_WhenDbSetThrows_EmitsErrorLog()
    {
        _mockContext.Setup(c => c.Set<Booking>()).Throws(new Exception("DB read failure"));

        var ex = Assert.ThrowsException<Exception>(() => _repository.GetOneByExpression(b => true));
        Assert.IsTrue(ex.Message.Contains("DB read failure"));

        _logManagerMock.Verify(m => m.Create(
                LogLevel.Error,
                "Failed to read from repository",
                It.Is<string>(s => s.Contains("DB read failure") || s.Contains(nameof(Booking)))),
            Times.Once);
    }

    [TestMethod]
    public void InsertOne_WhenDbSetThrows_EmitsErrorLog()
    {
        _mockContext.Setup(c => c.Set<Booking>()).Throws(new Exception("DB read failure"));

        var ex = Assert.ThrowsException<Exception>(() => _repository.InsertOne(_booking));
        Assert.IsTrue(ex.Message.Contains("DB read failure"));

        _logManagerMock.Verify(m => m.Create(
                LogLevel.Error,
                "Failed to prepare insert in repository",
                It.Is<string>(s => s.Contains("DB read failure") || s.Contains(nameof(Booking)))),
            Times.Once);
    }


    [TestMethod]
    public void DeleteOne_WhenDbSetThrows_EmitsErrorLog()
    {
        _mockContext.Setup(c => c.Set<Booking>()).Throws(new Exception("DB read failure"));

        var ex = Assert.ThrowsException<Exception>(() => _repository.DeleteOne(_booking));
        Assert.IsTrue(ex.Message.Contains("DB read failure"));

        _logManagerMock.Verify(m => m.Create(
                LogLevel.Error,
                "Failed to prepare delete in repository",
                It.Is<string>(s => s.Contains("DB read failure") || s.Contains(nameof(Booking)))),
            Times.Once);
    }

    [TestMethod]
    public void UpdateOne_WhenDbSetThrows_EmitsErrorLog()
    {
        _mockContext.Setup(c => c.Set<Booking>()).Throws(new Exception("DB read failure"));

        var ex = Assert.ThrowsException<Exception>(() => _repository.UpdateOne(_booking));
        Assert.IsTrue(ex.Message.Contains("DB read failure"));

        _logManagerMock.Verify(m => m.Create(
                LogLevel.Error,
                "Failed to prepare update in repository",
                It.Is<string>(s => s.Contains("DB read failure") || s.Contains(nameof(Booking)))),
            Times.Once);
    }

    [TestMethod]
    public void ExistsWithObject_WhenDbSetThrows_EmitsErrorLog()
    {
        _mockContext.Setup(c => c.Set<Booking>()).Throws(new Exception("DB read failure"));

        var ex = Assert.ThrowsException<Exception>(() => _repository.Exists(_booking));
        Assert.IsTrue(ex.Message.Contains("DB read failure"));

        _logManagerMock.Verify(m => m.Create(
                LogLevel.Error,
                "Failed to read from repository",
                It.Is<string>(s => s.Contains("DB read failure") || s.Contains(nameof(Booking)))),
            Times.Once);
    }

    [TestMethod]
    public void ExistsWithExpression_WhenDbSetThrows_EmitsErrorLog()
    {
        _mockContext.Setup(c => c.Set<Booking>()).Throws(new Exception("DB read failure"));

        var ex = Assert.ThrowsException<Exception>(() => _repository.Exists(b => b.BookerEmail == "a@gmail.com"));
        Assert.IsTrue(ex.Message.Contains("DB read failure"));

        _logManagerMock.Verify(m => m.Create(
                LogLevel.Error,
                "Failed to read from repository",
                It.Is<string>(s => s.Contains("DB read failure") || s.Contains(nameof(Booking)))),
            Times.Once);
    }

    [TestMethod]
    public void GetAllBasicByExpression_WhenDbSetThrows_EmitsErrorLog()
    {
        _mockContext.Setup(c => c.Set<Booking>()).Throws(new Exception("DB read failure"));

        var ex = Assert.ThrowsException<Exception>(() => _repository.GetAllBasicByExpression(b => b.BookerEmail == "a@gmail.com"));
        Assert.IsTrue(ex.Message.Contains("DB read failure"));

        _logManagerMock.Verify(m => m.Create(
                LogLevel.Error,
                "Failed to read from repository",
                It.Is<string>(s => s.Contains("DB read failure") || s.Contains(nameof(Booking)))),
            Times.Once);
    }

    [TestMethod]
    public void GetOneDetailByExpression_WhenDbSetThrows_EmitsErrorLog()
    {
        _mockContext.Setup(c => c.Set<Booking>()).Throws(new Exception("DB read failure"));

        var ex = Assert.ThrowsException<Exception>(() => _repository.GetOneDetailByExpression(b => b.BookerEmail == "a@gmail.com"));
        Assert.IsTrue(ex.Message.Contains("DB read failure"));

        _logManagerMock.Verify(m => m.Create(
                LogLevel.Error,
                "Failed to read from repository",
                It.Is<string>(s => s.Contains("DB read failure") || s.Contains(nameof(Booking)))),
            Times.Once);
    }
}