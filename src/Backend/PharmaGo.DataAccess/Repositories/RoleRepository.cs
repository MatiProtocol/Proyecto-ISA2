using Microsoft.EntityFrameworkCore;
using PharmaGo.Domain.Entities;
using PharmaGo.IBusinessLogic;

namespace PharmaGo.DataAccess.Repositories
{
    public class RoleRepository : BaseRepository<Role>
    {

        PharmacyGoDbContext _context;
        private DbSet<Role> _roles;
        public RoleRepository(PharmacyGoDbContext context, ILogManager logManager) : base(context, logManager)
        {
            _context = context;
        }
    }
}
