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
    public class ProductStockConfiguration : IEntityTypeConfiguration<ProductStock>
    {
        public void Configure(EntityTypeBuilder<ProductStock> builder)
        {
            builder.ToTable("ProductStocks");

            builder.HasKey(ps => new { ps.ProductId, ps.WarehouseId });

            // Decimal properties
            builder.Property(ps => ps.AverageCost).HasPrecision(18, 2);
            builder.Property(ps => ps.LastPurchasePrice).HasPrecision(18, 2);

            // Foreign Keys
            builder.HasOne(ps => ps.Product)
                .WithMany(p => p.ProductStocks)
                .HasForeignKey(ps => ps.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Product silinince stok da silinsin

            builder.HasOne(ps => ps.Warehouse)
                .WithMany(w => w.ProductStocks)
                .HasForeignKey(ps => ps.WarehouseId)
                .OnDelete(DeleteBehavior.Cascade); // Warehouse silinince stok da silinsin

            // Indexes
            builder.HasIndex(ps => ps.CurrentStock);
            builder.HasIndex(ps => ps.LastMovementDate);
        }
    }
}
