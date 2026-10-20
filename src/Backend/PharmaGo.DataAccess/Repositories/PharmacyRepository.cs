using Microsoft.EntityFrameworkCore;
using PharmaGo.Domain.Entities;
using System.Linq.Expressions;
using PharmaGo.IBusinessLogic;

namespace PharmaGo.DataAccess.Repositories
{
    public class PharmacyRepository : BaseRepository<Pharmacy>
    {
        PharmacyGoDbContext _context;

        public PharmacyRepository(PharmacyGoDbContext context, ILogManager logManager) : base(context, logManager)
        {
            _context = context;
        }

        public override Pharmacy GetOneByExpression(Expression<Func<Pharmacy, bool>> expression)
        {
            return _context.Set<Pharmacy>().Include("Users").Include("Drugs").FirstOrDefault(expression);
        }
    }
}
