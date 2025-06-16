using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Stoocker.Application.Interfaces.Repositories;
using Stoocker.Application.Interfaces.Repositories.Entities.Brand;
using Stoocker.Application.Interfaces.Repositories.Specification;
using Stoocker.Persistence.Contexts;
using Stoocker.Persistence.Repositories.Entities.Brand.Specifications;
using Stoocker.Persistence.Repositories.Specification;

namespace Stoocker.Persistence.Repositories.Entities.Brand
{
    public class BrandReadRepository : SpecificationRepository<Domain.Entities.Brand>, IBrandReadRepository
    {
        public BrandReadRepository(ApplicationDbContext context) : base(context)
        {
             
        }


        public async Task<IEnumerable<Domain.Entities.Brand>> GetBrandsByTenantIdAsync(Guid tenantId, string searchTerm, int page = 0, int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            try
            { 
                if (tenantId == Guid.Empty)
                    throw new ArgumentException("TenantId cannot be empty", nameof(tenantId));

                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                var spec = new BrandSearchSpecification(tenantId, searchTerm, page, pageSize);
                return await FindAsync(spec, cancellationToken);
            }
            catch (Exception ex)
            { 
                Console.WriteLine($"Error in GetBrandsByTenantIdAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<Domain.Entities.Brand> GetBrandsById(Guid Id, Guid tenantId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (tenantId == Guid.Empty)
                    throw new ArgumentException("TenantId cannot be empty", nameof(tenantId));
                var spec=new BrandByIdSpecification(Id, tenantId);
                return await FirstOrDefaultAsync(spec,cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBrandsById: {ex.Message}");
                throw;
            }
        }
    }
}
