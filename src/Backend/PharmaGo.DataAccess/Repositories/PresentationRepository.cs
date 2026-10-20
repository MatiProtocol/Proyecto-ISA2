using PharmaGo.Domain.Entities;
using PharmaGo.IBusinessLogic;

namespace PharmaGo.DataAccess.Repositories
{
    public class PresentationRepository : BaseRepository<Presentation>
    {
        private readonly PharmacyGoDbContext _context;

        public PresentationRepository(PharmacyGoDbContext context, ILogManager logManager) : base(context, logManager)
        {
            _context = context;
        }
    }
}
