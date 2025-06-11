using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Application.Interfaces.Repositories.Entities.SalesInvoice;
using Stoocker.Persistence.Contexts;

namespace Stoocker.Persistence.Repositories.Entities.SalesInvoice
{
    public class SalesInvoiceReadRepository:ReadRepository<Domain.Entities.SalesInvoice>,ISalesInvoiceReadRepository
    {
        public SalesInvoiceReadRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
