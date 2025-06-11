using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Application.Interfaces.Repositories.Entities.PurchaseInvoiceDetail;
using Stoocker.Persistence.Contexts;

namespace Stoocker.Persistence.Repositories.Entities.PurchaseInvoiceDetail
{
    public class PurchaseInvoiceDetailReadRepository:ReadRepository<Domain.Entities.PurchaseInvoiceDetail>,IPurchaseInvoiceDetailReadRepository
    {
        public PurchaseInvoiceDetailReadRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
