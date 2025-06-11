using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Application.Interfaces.Repositories.Entities.StockMovement;
using Stoocker.Persistence.Contexts;

namespace Stoocker.Persistence.Repositories.Entities.StockMovement
{
    public class StockMovementWriteRepository:WriteRepository<Domain.Entities.StockMovement>,IStockMovementWriteRepository
    {
        public StockMovementWriteRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
