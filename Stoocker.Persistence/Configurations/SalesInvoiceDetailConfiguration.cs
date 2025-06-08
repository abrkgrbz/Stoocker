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
    public class SalesInvoiceDetailConfiguration : IEntityTypeConfiguration<SalesInvoiceDetail>
    {
        public void Configure(EntityTypeBuilder<SalesInvoiceDetail> builder)
        {
            builder.ToTable("SalesInvoiceDetails");

            // Decimal properties
            builder.Property(sid => sid.UnitPrice).HasPrecision(18, 2);
            builder.Property(sid => sid.DiscountRate).HasPrecision(5, 2);
            builder.Property(sid => sid.DiscountAmount).HasPrecision(18, 2);
            builder.Property(sid => sid.TaxRate).HasPrecision(5, 2);
            builder.Property(sid => sid.TaxAmount).HasPrecision(18, 2);
            builder.Property(sid => sid.LineTotal).HasPrecision(18, 2);

            builder.Property(sid => sid.Description).HasMaxLength(500);

            // Foreign Keys
            builder.HasOne(sid => sid.Invoice)
                .WithMany(si => si.Details)
                .HasForeignKey(sid => sid.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade); // Invoice silinince detail da silinsin

            builder.HasOne(sid => sid.Product)
                .WithMany(p => p.SalesInvoiceDetails)
                .HasForeignKey(sid => sid.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // Product silinmesin

            // Indexes
            builder.HasIndex(sid => new { sid.InvoiceId, sid.LineNumber });
            builder.HasIndex(sid => sid.ProductId);
        }
    }
}
