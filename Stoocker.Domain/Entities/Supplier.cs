using Stoocker.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Domain.Enums;

namespace Stoocker.Domain.Entities
{
    public class Supplier : BaseEntity, ITenantEntity
    {
        public Guid TenantId { get; set; }
        public string SupplierCode { get; set; } = string.Empty; // Auto-generated
        public string Name { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public string? Fax { get; set; }
        public string? Website { get; set; }

        // Adres Bilgileri
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; } = "Turkey";

        // Mali Bilgiler
        public string? TaxNumber { get; set; }
        public string? TaxOffice { get; set; }
        public int PaymentTerms { get; set; } = 30; // Ödeme vadesi (gün)
        public decimal CurrentBalance { get; set; } = 0;
        public decimal TotalPurchases { get; set; } = 0;
        public DateTime? LastPurchaseDate { get; set; }

        // Banka Bilgileri
        public string? BankName { get; set; }
        public string? BankBranch { get; set; }
        public string? AccountNumber { get; set; }
        public string? IBAN { get; set; }

        // Durum ve Ayarlar
        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }
        public string? Tags { get; set; } // Comma separated
        public SupplierRating Rating { get; set; } = SupplierRating.Good;

        // Navigation Properties
        public virtual Tenant Tenant { get; set; } = null!;
        public virtual ICollection<PurchaseInvoice> PurchaseInvoices { get; set; } = new List<PurchaseInvoice>();
    }
}
