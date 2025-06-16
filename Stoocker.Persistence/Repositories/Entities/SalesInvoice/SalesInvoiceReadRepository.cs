using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Application.Interfaces.Repositories.Entities.SalesInvoice;
using Stoocker.Persistence.Contexts;
using Stoocker.Persistence.Repositories.Specification;

namespace Stoocker.Persistence.Repositories.Entities.SalesInvoice
{
    public class SalesInvoiceReadRepository: SpecificationRepository<Domain.Entities.SalesInvoice>,ISalesInvoiceReadRepository
    {
        public SalesInvoiceReadRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
