using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Application.Interfaces.Repositories.Entities.Supplier;
using Stoocker.Persistence.Contexts;

namespace Stoocker.Persistence.Repositories.Entities.Supplier
{
    public class SupplierReadRepository:ReadRepository<Domain.Entities.Supplier>,ISupplierReadRepository
    {
        public SupplierReadRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
