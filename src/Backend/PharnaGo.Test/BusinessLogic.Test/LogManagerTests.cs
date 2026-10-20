using Moq;
using PharmaGo.BusinessLogic;
using PharmaGo.Domain.Entities;
using PharmaGo.Domain.Enums;
using PharmaGo.Exceptions;
using PharmaGo.IBusinessLogic;
using PharmaGo.IDataAccess;

namespace PharmaGo.Test.BusinessLogic.Test;

[TestClass]
public class LogManagerTests
{
    private ILogManager _logManager;
    private LogLevel _logWarning;
    private string _message;
    private Mock<IRepository<Log>> _logRepository;

    [TestInitialize]
    public void Initialize()
    {
        _logRepository = new Mock<IRepository<Log>>();
        _logManager = new LogManager(_logRepository.Object);

        _logWarning = LogLevel.Warning;
        _message = "_message";


    }

    [TestMethod]
    public void Create_ReturnsCorrectType()
    {
        var response = _logManager.Create(_logWarning, _message, null);

        Assert.IsInstanceOfType(response, typeof(Log));
    }

    [TestMethod]
    public void Create_ReturnsLog_WithCorrectInformation()
    {
        var dateNow = DateTime.UtcNow;

        var response = _logManager.Create(_logWarning, _message, null);
        var dateDifference = Math.Abs((response.Timestamp - dateNow).TotalSeconds);

        Assert.IsTrue(dateDifference < 1.0);
        Assert.AreEqual(response.Level, _logWarning);
        Assert.AreEqual(response.Message, _message);
        Assert.AreEqual(response.AdditionalData, null);
    }

    [TestMethod]
    public void Create_Log_CallsInsertOne()
    {
        _logManager.Create(_logWarning, _message, null);

        _logRepository!.Verify(m => m.InsertOne(It.IsAny<Log>()), Times.Once);
    }

    [TestMethod]
    public void Create_Log_FailEmptyMessage()
    {
        _message = "";

        var message = Assert.ThrowsException<InvalidResourceException>(() => _logManager.Create(_logWarning, _message, null)).Message;

        Assert.AreEqual("Invalid message", message);
    }
}