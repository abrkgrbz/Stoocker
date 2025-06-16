using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Application.Interfaces.Repositories.Entities.StockMovement;
using Stoocker.Persistence.Contexts;
using Stoocker.Persistence.Repositories.Specification;

namespace Stoocker.Persistence.Repositories.Entities.StockMovement
{
    public class StockMovementReadRepository: SpecificationRepository<Domain.Entities.StockMovement>,IStockMovementReadRepository
    {
        public StockMovementReadRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
