using Microsoft.EntityFrameworkCore;
using PharmaGo.Domain.Entities;
using System.Linq.Expressions;
using PharmaGo.IBusinessLogic;

namespace PharmaGo.DataAccess.Repositories
{
    public class PurchasesDetailRepository : BaseRepository<PurchaseDetail>
    {
        private readonly PharmacyGoDbContext _context;

        public PurchasesDetailRepository(PharmacyGoDbContext context, ILogManager logManager) : base(context, logManager)
        {
            _context = context;
        }

        public override bool Exists(PurchaseDetail element)
        {
            bool exists = false;
            exists = _context.Set<PurchaseDetail>().Any<PurchaseDetail>(e => e.Id == element.Id);
            return exists;
        }

        public override PurchaseDetail GetOneByExpression(Expression<Func<PurchaseDetail, bool>> expression)
        {
            return _context.Set<PurchaseDetail>()
                .Include(x => x.Pharmacy)
                .FirstOrDefault(expression);
        }
    }
}
