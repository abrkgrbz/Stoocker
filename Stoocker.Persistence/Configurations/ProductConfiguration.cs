using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stoocker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Persistence.Configurations
{
    public class ProductConfiguration: IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.ProductCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(p => p.Description)
                .HasMaxLength(1000);

            // Decimal precision
            builder.Property(p => p.PurchasePrice)
                .HasPrecision(18, 2);

            builder.Property(p => p.SalePrice)
                .HasPrecision(18, 2);

            builder.Property(p => p.WholesalePrice)
                .HasPrecision(18, 2);

            builder.Property(p => p.MinSalePrice)
                .HasPrecision(18, 2);

            builder.Property(p => p.Weight)
                .HasPrecision(18, 3);

            builder.Property(p => p.Volume)
                .HasPrecision(18, 3);

            builder.Property(p => p.TaxRate)
                .HasPrecision(5, 2)
                .HasDefaultValue(18.00m);

            // Foreign Keys - CASCADE DELETE KAPALI
            builder.HasOne(p => p.Tenant)
                .WithMany(t => t.Products)
                .HasForeignKey(p => p.TenantId)
                .OnDelete(DeleteBehavior.Restrict); // NO ACTION

            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // NO ACTION

            builder.HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.SetNull); // Brand silinince null yap

            builder.HasOne(p => p.Unit)
                .WithMany(u => u.Products)
                .HasForeignKey(p => p.UnitId)
                .OnDelete(DeleteBehavior.Restrict); // NO ACTION

            // Indexes
            builder.HasIndex(p => new { p.TenantId, p.ProductCode })
                .IsUnique();

            builder.HasIndex(p => p.Barcode)
                .HasFilter("[Barcode] IS NOT NULL");

            builder.HasIndex(p => new { p.TenantId, p.IsActive });
            builder.HasIndex(p => new { p.TenantId, p.CategoryId });
        }
    }
}
