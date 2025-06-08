using Stoocker.Domain.Common;
using Stoocker.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Domain.Entities
{
    public class StockMovement : BaseEntity, ITenantEntity
    {
        public Guid TenantId { get; set; }
        public Guid ProductId { get; set; }
        public Guid WarehouseId { get; set; }
        public StockMovementType MovementType { get; set; }
        public int Quantity { get; set; } // + giriş, - çıkış
        public decimal UnitPrice { get; set; } = 0;
        public decimal TotalAmount => Math.Abs(Quantity) * UnitPrice;
        public string? Reference { get; set; } // Fatura no, transfer no vs.
        public string? Description { get; set; }
        public DateTime MovementDate { get; set; } = DateTime.UtcNow;
        public Guid? RelatedMovementId { get; set; } // Transfer için karşı hareket
        public string? BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? SerialNumber { get; set; }

        // İlgili belgeler
        public Guid? SalesInvoiceId { get; set; }
        public Guid? PurchaseInvoiceId { get; set; }
        public Guid? TransferId { get; set; }

        // Navigation Properties
        public virtual Tenant Tenant { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
        public virtual Warehouse Warehouse { get; set; } = null!;
        public virtual ApplicationUser CreatedByUser { get; set; } = null!;
        public virtual SalesInvoice? SalesInvoice { get; set; }
        public virtual PurchaseInvoice? PurchaseInvoice { get; set; }
    }

}
