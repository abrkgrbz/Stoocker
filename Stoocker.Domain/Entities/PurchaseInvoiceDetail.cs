using Stoocker.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Domain.Entities
{
    public class PurchaseInvoiceDetail : BaseEntity
    {
        public Guid InvoiceId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountRate { get; set; } = 0;
        public decimal DiscountAmount { get; set; } = 0;
        public decimal TaxRate { get; set; } = 18;
        public decimal TaxAmount { get; set; } = 0;
        public decimal LineTotal { get; set; } = 0;
        public int LineNumber { get; set; }
        public string? Description { get; set; }
        public string? SerialNumbers { get; set; } // JSON array
        public string? BatchNumbers { get; set; } // JSON array
        public DateTime? ExpiryDate { get; set; }

        // Navigation Properties
        public virtual PurchaseInvoice Invoice { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }

}
