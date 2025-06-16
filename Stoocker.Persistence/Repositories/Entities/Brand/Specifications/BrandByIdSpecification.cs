using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Persistence.Repositories.Specification;

namespace Stoocker.Persistence.Repositories.Entities.Brand.Specifications
{
    public class BrandByIdSpecification:BaseSpecification<Domain.Entities.Brand>
    {
        public BrandByIdSpecification(Guid id,Guid tenantId)
        {
            Criteria = b => b.Id == id && b.TenantId == tenantId;

        }
    }
}
