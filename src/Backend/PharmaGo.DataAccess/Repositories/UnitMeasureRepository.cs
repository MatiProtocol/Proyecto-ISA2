using PharmaGo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PharmaGo.IBusinessLogic;

namespace PharmaGo.DataAccess.Repositories
{
    public class UnitMeasureRepository : BaseRepository<UnitMeasure>
    {
        private readonly PharmacyGoDbContext _context;

        public UnitMeasureRepository(PharmacyGoDbContext context, ILogManager logManager) : base(context, logManager)
        {
            _context = context;
        }
    }
}
