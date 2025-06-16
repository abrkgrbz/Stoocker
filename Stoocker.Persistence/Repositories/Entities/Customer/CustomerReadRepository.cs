using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Application.Interfaces.Repositories.Entities.Customer;
using Stoocker.Persistence.Contexts;
using Stoocker.Persistence.Repositories.Specification;

namespace Stoocker.Persistence.Repositories.Entities.Customer
{
    public class CustomerReadRepository: SpecificationRepository<Domain.Entities.Customer>,ICustomerReadRepository
    {
        public CustomerReadRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
