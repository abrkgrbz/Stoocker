using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Application.Interfaces.Repositories.Entities.Product;
using Stoocker.Persistence.Contexts;

namespace Stoocker.Persistence.Repositories.Entities.Product
{
    public class ProductReadRepository:ReadRepository<Domain.Entities.Product>,IProductReadRepository
    {
        public ProductReadRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
