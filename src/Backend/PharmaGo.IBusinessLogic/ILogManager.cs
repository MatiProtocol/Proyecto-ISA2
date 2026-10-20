using PharmaGo.Domain.Entities;
using PharmaGo.Domain.Enums;

namespace PharmaGo.IBusinessLogic;

public interface ILogManager
{
    public Log Create(LogLevel level, string message, string? additionalData);
}