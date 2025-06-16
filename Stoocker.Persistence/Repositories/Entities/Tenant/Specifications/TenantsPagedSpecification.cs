using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Persistence.Repositories.Specification;

namespace Stoocker.Persistence.Repositories.Entities.Tenant.Specifications
{
    public class TenantsPagedSpecification:BaseSpecification<Domain.Entities.Tenant>
    {
        public TenantsPagedSpecification(Guid tenantId, int page = 1, int pageSize = 10)
        { 
            if (page > 0 && pageSize > 0)
            {
                ApplyPaging((page - 1) * pageSize, pageSize);
            }
             
            ApplyOrderByDescending(b => b.CreatedAt);
        }
    }
}
