using Stoocker.Domain.Common;
using Stoocker.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Domain.Entities
{
    public class SalesInvoice : BaseEntity, ITenantEntity
    {
        public Guid TenantId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty; // Auto-generated
        public Guid CustomerId { get; set; }
        public Guid WarehouseId { get; set; }
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public DateTime? ShippingDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        // Tutarlar
        public decimal SubTotal { get; set; } = 0;
        public decimal TaxAmount { get; set; } = 0;
        public decimal DiscountAmount { get; set; } = 0;
        public decimal ShippingAmount { get; set; } = 0;
        public decimal TotalAmount { get; set; } = 0;
        public decimal PaidAmount { get; set; } = 0;
        public decimal RemainingAmount => TotalAmount - PaidAmount;

        // Durum
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

        // Notlar ve Açıklamalar
        public string? Notes { get; set; }
        public string? InternalNotes { get; set; }
        public string? ShippingAddress { get; set; }
        public string? TrackingNumber { get; set; }

        // Ödeme Bilgileri
        public DateTime? PaidDate { get; set; }
        public string? PaymentReference { get; set; }

        // Komisyon ve İndirimler
        public decimal CommissionRate { get; set; } = 0;
        public decimal CommissionAmount { get; set; } = 0;
        public string? CampaignCode { get; set; }

        // Navigation Properties
        public virtual Tenant Tenant { get; set; } = null!;
        public virtual Customer Customer { get; set; } = null!;
        public virtual Warehouse Warehouse { get; set; } = null!;
        public virtual ApplicationUser CreatedByUser { get; set; } = null!;
        public virtual ICollection<SalesInvoiceDetail> Details { get; set; } = new List<SalesInvoiceDetail>();
        public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    }
}
