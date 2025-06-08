using Stoocker.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Domain.Enums;

namespace Stoocker.Domain.Entities
{
    public class Warehouse : BaseEntity, ITenantEntity
    {
        public Guid TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty; // Depo kodu
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; } = "Turkey";
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public bool IsDefault { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public string? ManagerName { get; set; }
        public Guid? ManagerUserId { get; set; }
        public WarehouseType Type { get; set; } = WarehouseType.Standard;

        // Kapasiteler
        public decimal? TotalArea { get; set; } // m²
        public decimal? StorageCapacity { get; set; } // m³
        public int? MaxProductCount { get; set; }

        public virtual Tenant Tenant { get; set; } = null!;
        public virtual ApplicationUser? Manager { get; set; }
        public virtual ICollection<ProductStock> ProductStocks { get; set; } = new List<ProductStock>();
        public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
        public virtual ICollection<SalesInvoice> SalesInvoices { get; set; } = new List<SalesInvoice>();
        public virtual ICollection<PurchaseInvoice> PurchaseInvoices { get; set; } = new List<PurchaseInvoice>();
    }
}
