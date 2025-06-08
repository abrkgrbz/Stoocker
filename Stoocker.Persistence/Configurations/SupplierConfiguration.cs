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
    public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.ToTable("Suppliers");

            builder.Property(s => s.SupplierCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.Email)
                .HasMaxLength(200);

            builder.Property(s => s.Phone)
                .HasMaxLength(20);

            builder.Property(s => s.TaxNumber)
                .HasMaxLength(50);

            builder.Property(s => s.IBAN)
                .HasMaxLength(50);

            // Decimal properties
            builder.Property(s => s.CurrentBalance).HasPrecision(18, 2);
            builder.Property(s => s.TotalPurchases).HasPrecision(18, 2);

            // Tenant relationship
            builder.HasOne(s => s.Tenant)
                .WithMany(t => t.Suppliers)
                .HasForeignKey(s => s.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(s => new { s.TenantId, s.SupplierCode })
                .IsUnique();

            builder.HasIndex(s => s.TaxNumber)
                .HasFilter("[TaxNumber] IS NOT NULL");
        }
    }

}
