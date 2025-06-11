using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Application.Interfaces.Repositories.Entities.PurchaseInvoice;
using Stoocker.Application.Interfaces.Repositories.Entities.PurchaseInvoiceDetail;
using Stoocker.Persistence.Contexts;

namespace Stoocker.Persistence.Repositories.Entities.PurchaseInvoiceDetail
{
    public class PurchaseInvoiceDetailWriteRepository:WriteRepository<Domain.Entities.PurchaseInvoiceDetail>,IPurchaseInvoiceDetailWriteRepository
    {
        public PurchaseInvoiceDetailWriteRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
