using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Domain.Common;
using Stoocker.Domain.Enums;

namespace Stoocker.Domain.Entities
{
    public class Tenant:BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Domain { get; set; }
        public string? ConnectionString { get; set; }
        public bool IsActive { get; set; } = true;
        public TenantPlan PlanType { get; set; } = TenantPlan.Basic;
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public int MaxUsers { get; set; } = 5;
        public int MaxProducts { get; set; } = 1000;
        public int MaxWarehouses { get; set; } = 1;
        public bool AllowMultiWarehouse { get; set; } = false;
        public bool AllowAdvancedReports { get; set; } = false;
        public bool AllowApiAccess { get; set; } = false;
         
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; } = "Turkey";
        public string? TaxNumber { get; set; }
        public string? TaxOffice { get; set; }
         
        public string? LogoPath { get; set; }
        public string? PrimaryColor { get; set; } = "#3498db";
        public string? SecondaryColor { get; set; } = "#2c3e50";
         
        public string? Settings { get; set; } // JSON

        public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public virtual ICollection<ApplicationRole> Roles { get; set; } = new List<ApplicationRole>();
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public virtual ICollection<Warehouse> Warehouses { get; set; } = new List<Warehouse>();
        public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
        public virtual ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();
        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
        public virtual ICollection<Brand> Brands { get; set; } = new List<Brand>();
        public virtual ICollection<Unit> Units { get; set; } = new List<Unit>();
    }
}
