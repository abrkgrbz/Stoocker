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
    public class SalesInvoiceConfiguration : IEntityTypeConfiguration<SalesInvoice>
    {
        public void Configure(EntityTypeBuilder<SalesInvoice> builder)
        {
            builder.ToTable("SalesInvoices");

            builder.Property(si => si.InvoiceNumber)
                .IsRequired()
                .HasMaxLength(50);

            // Decimal properties
            builder.Property(si => si.SubTotal).HasPrecision(18, 2);
            builder.Property(si => si.TaxAmount).HasPrecision(18, 2);
            builder.Property(si => si.DiscountAmount).HasPrecision(18, 2);
            builder.Property(si => si.ShippingAmount).HasPrecision(18, 2);
            builder.Property(si => si.TotalAmount).HasPrecision(18, 2);
            builder.Property(si => si.PaidAmount).HasPrecision(18, 2);
            builder.Property(si => si.CommissionRate).HasPrecision(5, 2);
            builder.Property(si => si.CommissionAmount).HasPrecision(18, 2);

            // Foreign Keys - CASCADE DELETE KAPALI
            builder.HasOne(si => si.Tenant)
                .WithMany()
                .HasForeignKey(si => si.TenantId)
                .OnDelete(DeleteBehavior.Restrict); // NO ACTION

            builder.HasOne(si => si.Customer)
                .WithMany(c => c.SalesInvoices)
                .HasForeignKey(si => si.CustomerId)
                .OnDelete(DeleteBehavior.Restrict); // NO ACTION

            builder.HasOne(si => si.Warehouse)
                .WithMany(w => w.SalesInvoices)
                .HasForeignKey(si => si.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict); // NO ACTION

            builder.HasOne(si => si.CreatedByUser)
                .WithMany(u => u.SalesInvoices)
                .HasForeignKey(si => si.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull) // User silinince null yap
                .IsRequired(false); // Nullable foreign key

            // Indexes
            builder.HasIndex(si => new { si.TenantId, si.InvoiceNumber })
                .IsUnique();

            builder.HasIndex(si => new { si.TenantId, si.CustomerId });
            builder.HasIndex(si => new { si.TenantId, si.InvoiceDate });
        }
    }
}
