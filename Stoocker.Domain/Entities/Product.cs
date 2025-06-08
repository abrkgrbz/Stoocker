using Stoocker.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Domain.Entities
{
    public class Product : BaseEntity, ITenantEntity
    {
        public Guid TenantId { get; set; }
        public string ProductCode { get; set; } = string.Empty;    
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid CategoryId { get; set; }
        public Guid? BrandId { get; set; }
        public Guid UnitId { get; set; }
         
        public decimal PurchasePrice { get; set; } = 0;  
        public decimal SalePrice { get; set; } = 0;  
        public decimal? WholesalePrice { get; set; }  
        public decimal? MinSalePrice { get; set; }  
         
        public int MinimumStock { get; set; } = 0;
        public int MaximumStock { get; set; } = 0;
        public int ReorderLevel { get; set; } = 0;
        public int ReorderQuantity { get; set; } = 0;
         
        public decimal? Weight { get; set; } // Kilogram
        public decimal? Volume { get; set; } // Litre
        public string? Dimensions { get; set; } // "L x W x H cm"
         
        public string? Barcode { get; set; }
        public string? QRCode { get; set; }
        public string? ManufacturerCode { get; set; }
        public string? SupplierCode { get; set; }
         
        public decimal TaxRate { get; set; } = 18; // KDV oranı
        public bool IsService { get; set; } = false;
        public bool IsDigitalProduct { get; set; } = false;
         
        public bool IsActive { get; set; } = true;
        public bool AllowNegativeStock { get; set; } = false;
        public bool TrackSerial { get; set; } = false;
        public bool TrackExpiry { get; set; } = false;
        public bool TrackBatches { get; set; } = false;
        public bool IsFavorite { get; set; } = false;
         
        public string? ImagePath { get; set; }
        public string? GalleryImages { get; set; } // JSON array of image paths
        public string? DocumentPath { get; set; }
         
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? Tags { get; set; } // Comma separated
         
        public DateTime? DiscontinuedDate { get; set; }
        public DateTime? LastSaleDate { get; set; }
        public DateTime? LastPurchaseDate { get; set; }

        public virtual Tenant Tenant { get; set; } = null!;
        public virtual Category Category { get; set; } = null!;
        public virtual Brand? Brand { get; set; }
        public virtual Unit Unit { get; set; } = null!;
        public virtual ICollection<ProductStock> ProductStocks { get; set; } = new List<ProductStock>();
        public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
        public virtual ICollection<SalesInvoiceDetail> SalesInvoiceDetails { get; set; } = new List<SalesInvoiceDetail>();
        public virtual ICollection<PurchaseInvoiceDetail> PurchaseInvoiceDetails { get; set; } = new List<PurchaseInvoiceDetail>();

    }
}
