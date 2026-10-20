using PharmaGo.Domain.Entities;
using PharmaGo.IBusinessLogic;

namespace PharmaGo.DataAccess.Repositories;

public class LogRepository : BaseRepository<Log>
{
    private readonly PharmacyGoDbContext _context;
    public LogRepository(PharmacyGoDbContext context, ILogManager logManager) : base(context, logManager) { _context = context; }
}