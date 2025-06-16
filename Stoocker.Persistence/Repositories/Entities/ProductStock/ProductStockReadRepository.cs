using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Application.Interfaces.Repositories.Entities.ProductStock;
using Stoocker.Persistence.Contexts;
using Stoocker.Persistence.Repositories.Specification;

namespace Stoocker.Persistence.Repositories.Entities.ProductStock
{
    public class ProductStockReadRepository: SpecificationRepository<Domain.Entities.ProductStock>,IProductStockReadRepository
    {
        public ProductStockReadRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
