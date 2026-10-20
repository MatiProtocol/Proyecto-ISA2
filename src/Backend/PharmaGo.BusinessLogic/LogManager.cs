using PharmaGo.Domain.Entities;
using PharmaGo.Domain.Enums;
using PharmaGo.Exceptions;
using PharmaGo.IBusinessLogic;
using PharmaGo.IDataAccess;

namespace PharmaGo.BusinessLogic;

public class LogManager(IRepository<Log> _logRepository) : ILogManager
{
    public Log Create(LogLevel level, string message, string? additionalData)
    {
        if (message == "") throw new InvalidResourceException("Invalid message");
        var log = new Log() { Level = level, Message = message, AdditionalData = additionalData };
        _logRepository.InsertOne(log);
        _logRepository.Save();
        return log;
    }
}