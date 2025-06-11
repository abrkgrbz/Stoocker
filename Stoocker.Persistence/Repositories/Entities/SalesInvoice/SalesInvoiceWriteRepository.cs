using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Application.Interfaces.Repositories.Entities.SalesInvoice;
using Stoocker.Persistence.Contexts;

namespace Stoocker.Persistence.Repositories.Entities.SalesInvoice
{
    public class SalesInvoiceWriteRepository:WriteRepository<Domain.Entities.SalesInvoice>,ISalesInvoiceWriteRepository
    {
        public SalesInvoiceWriteRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
