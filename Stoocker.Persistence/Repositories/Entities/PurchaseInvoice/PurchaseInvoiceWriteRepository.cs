using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Application.Interfaces.Repositories.Entities.PurchaseInvoice;
using Stoocker.Persistence.Contexts;

namespace Stoocker.Persistence.Repositories.Entities.PurchaseInvoice
{
    public class PurchaseInvoiceWriteRepository:WriteRepository<Domain.Entities.PurchaseInvoice>,IPurchaseInvoiceWriteRepository
    {
        public PurchaseInvoiceWriteRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
