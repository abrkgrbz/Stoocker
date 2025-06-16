using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Application.Interfaces.Repositories.Entities.Warehouse;
using Stoocker.Persistence.Contexts;
using Stoocker.Persistence.Repositories.Specification;

namespace Stoocker.Persistence.Repositories.Entities.Warehouse
{
    public class WarehouseReadRepository: SpecificationRepository<Domain.Entities.Warehouse>,IWarehouseReadRepository
    {
        public WarehouseReadRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
