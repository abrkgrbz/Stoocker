using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Application.Interfaces.Repositories.Entities.Warehouse;
using Stoocker.Persistence.Contexts;

namespace Stoocker.Persistence.Repositories.Entities.Warehouse
{
    public class WarehouseReadRepository:ReadRepository<Domain.Entities.Warehouse>,IWarehouseReadRepository
    {
        public WarehouseReadRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
