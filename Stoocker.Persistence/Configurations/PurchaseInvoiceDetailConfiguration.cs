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
    public class PurchaseInvoiceDetailConfiguration : IEntityTypeConfiguration<PurchaseInvoiceDetail>
    {
        public void Configure(EntityTypeBuilder<PurchaseInvoiceDetail> builder)
        {
            builder.ToTable("PurchaseInvoiceDetails");

            // Decimal properties
            builder.Property(pid => pid.UnitPrice).HasPrecision(18, 2);
            builder.Property(pid => pid.DiscountRate).HasPrecision(5, 2);
            builder.Property(pid => pid.DiscountAmount).HasPrecision(18, 2);
            builder.Property(pid => pid.TaxRate).HasPrecision(5, 2);
            builder.Property(pid => pid.TaxAmount).HasPrecision(18, 2);
            builder.Property(pid => pid.LineTotal).HasPrecision(18, 2);

            builder.Property(pid => pid.Description).HasMaxLength(500);

            // Foreign Keys
            builder.HasOne(pid => pid.Invoice)
                .WithMany(pi => pi.Details)
                .HasForeignKey(pid => pid.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade); // Invoice silinince detail da silinsin

            builder.HasOne(pid => pid.Product)
                .WithMany(p => p.PurchaseInvoiceDetails)
                .HasForeignKey(pid => pid.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // Product silinmesin

            // Indexes
            builder.HasIndex(pid => new { pid.InvoiceId, pid.LineNumber });
            builder.HasIndex(pid => pid.ProductId);
        }
    }

}
