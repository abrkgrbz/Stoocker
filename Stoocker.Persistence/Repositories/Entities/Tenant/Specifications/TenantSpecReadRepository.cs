using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Application.Interfaces.Repositories;
using Stoocker.Application.Interfaces.Repositories.Entities.Tenant;
using Stoocker.Persistence.Contexts;
using Stoocker.Persistence.Repositories.Entities.Brand.Specifications;
using Stoocker.Persistence.Repositories.Specification;

namespace Stoocker.Persistence.Repositories.Entities.Tenant.Specifications
{
    public class TenantSpecReadRepository:SpecificationRepository<Domain.Entities.Tenant>, ITenantSpecReadRepository
    {
        public TenantSpecReadRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Domain.Entities.Tenant>> GetPagedAsync(Guid tenantId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            try
            {
                if (tenantId == Guid.Empty)
                    throw new ArgumentException("TenantId cannot be empty", nameof(tenantId));

                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                var spec = new TenantsPagedSpecification(tenantId, page, pageSize);
                return await FindAsync(spec, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in TE: {ex.Message}");
                throw;
            }
        }
    }
}
