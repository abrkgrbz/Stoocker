using Stoocker.Domain.Common;
using Stoocker.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Domain.Entities
{
    public class Customer : BaseEntity, ITenantEntity
    {
        public Guid TenantId { get; set; }
        public string CustomerCode { get; set; } = string.Empty; // Auto-generated
        public CustomerType Type { get; set; } = CustomerType.Individual;
        public string Name { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
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
        public string? IdentityNumber { get; set; } // TC Kimlik No
        public decimal CreditLimit { get; set; } = 0;
        public int PaymentTerms { get; set; } = 0; // Ödeme vadesi (gün)
        public decimal CurrentBalance { get; set; } = 0;
        public decimal TotalSales { get; set; } = 0;
        public DateTime? LastSaleDate { get; set; }

        // Durum ve Ayarlar
        public bool IsActive { get; set; } = true;
        public bool AllowCredit { get; set; } = true;
        public bool SendEmailInvoice { get; set; } = false;
        public bool SendSmsNotification { get; set; } = false;
        public string? Notes { get; set; }
        public string? Tags { get; set; } // Comma separated
        public CustomerPriority Priority { get; set; } = CustomerPriority.Normal;

        // Navigation Properties
        public virtual Tenant Tenant { get; set; } = null!;
        public virtual ICollection<SalesInvoice> SalesInvoices { get; set; } = new List<SalesInvoice>();
    }
}
