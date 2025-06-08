using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Stoocker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Persistence.Configurations
{
    public class PurchaseInvoiceConfiguration : IEntityTypeConfiguration<PurchaseInvoice>
    {
        public void Configure(EntityTypeBuilder<PurchaseInvoice> builder)
        {
            builder.ToTable("PurchaseInvoices");

            builder.Property(pi => pi.InvoiceNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(pi => pi.SupplierInvoiceNumber)
                .HasMaxLength(50);

            // Decimal properties
            builder.Property(pi => pi.SubTotal).HasPrecision(18, 2);
            builder.Property(pi => pi.TaxAmount).HasPrecision(18, 2);
            builder.Property(pi => pi.DiscountAmount).HasPrecision(18, 2);
            builder.Property(pi => pi.ShippingAmount).HasPrecision(18, 2);
            builder.Property(pi => pi.TotalAmount).HasPrecision(18, 2);
            builder.Property(pi => pi.PaidAmount).HasPrecision(18, 2);

            // Foreign Keys - CASCADE DELETE KAPALI
            builder.HasOne(pi => pi.Tenant)
                .WithMany()
                .HasForeignKey(pi => pi.TenantId)
                .OnDelete(DeleteBehavior.Restrict); // NO ACTION

            builder.HasOne(pi => pi.Supplier)
                .WithMany(s => s.PurchaseInvoices)
                .HasForeignKey(pi => pi.SupplierId)
                .OnDelete(DeleteBehavior.Restrict); // NO ACTION

            builder.HasOne(pi => pi.Warehouse)
                .WithMany(w => w.PurchaseInvoices)
                .HasForeignKey(pi => pi.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict); // NO ACTION

            builder.HasOne(pi => pi.CreatedByUser)
                .WithMany(u => u.PurchaseInvoices)
                .HasForeignKey(pi => pi.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull) // User silinince null yap
                .IsRequired(false); // Nullable foreign key

            // Indexes
            builder.HasIndex(pi => new { pi.TenantId, pi.InvoiceNumber })
                .IsUnique();

            builder.HasIndex(pi => new { pi.TenantId, pi.SupplierId });
            builder.HasIndex(pi => new { pi.TenantId, pi.InvoiceDate });
        }
    }

}
