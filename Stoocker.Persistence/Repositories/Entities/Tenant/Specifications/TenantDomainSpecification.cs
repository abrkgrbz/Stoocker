using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Persistence.Repositories.Specification;

namespace Stoocker.Persistence.Repositories.Entities.Tenant.Specifications
{
    public class TenantDomainSpecification: BaseSpecification<Domain.Entities.Tenant>
    {
        public TenantDomainSpecification(string domain)
        {
            Criteria = tenant => tenant.Domain == domain;
        }
    }
}
