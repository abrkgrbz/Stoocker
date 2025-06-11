using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Application.Interfaces.Repositories.Entities.Unit;
using Stoocker.Persistence.Contexts;

namespace Stoocker.Persistence.Repositories.Entities.Unit
{
    public class UnitReadRepository:ReadRepository<Domain.Entities.Unit>, IUnitReadRepository
    {
        public UnitReadRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
