using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Application.Interfaces.Repositories.Entities.Supplier;
using Stoocker.Persistence.Contexts;
using Stoocker.Persistence.Repositories.Specification;

namespace Stoocker.Persistence.Repositories.Entities.Supplier
{
    public class SupplierReadRepository: SpecificationRepository<Domain.Entities.Supplier>,ISupplierReadRepository
    {
        public SupplierReadRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
