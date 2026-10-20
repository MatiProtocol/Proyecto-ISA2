using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PharmaGo.Domain.Entities;
using PharmaGo.IBusinessLogic;

namespace PharmaGo.DataAccess.Repositories;

public class BookingRepository(PharmacyGoDbContext context, ILogManager logManager) : BaseRepository<Booking>(context, logManager)
{
    private readonly PharmacyGoDbContext _context = context;

    public override Booking GetOneByExpression(Expression<Func<Booking, bool>> expression)
    {
        return _context.Set<Booking>().Include("Details").FirstOrDefault(expression);
    }

    public override IEnumerable<Booking> GetAllByExpression(Expression<Func<Booking, bool>> expression)
    {
        return _context.Set<Booking>().Include("Details").Where(expression);
    }
}