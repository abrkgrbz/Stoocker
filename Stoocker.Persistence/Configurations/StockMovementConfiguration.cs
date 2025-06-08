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
    public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
    {
        public void Configure(EntityTypeBuilder<StockMovement> builder)
        {
            builder.ToTable("StockMovements");

            builder.Property(sm => sm.UnitPrice)
                .HasPrecision(18, 2);

            builder.Property(sm => sm.Reference)
                .HasMaxLength(100);

            builder.Property(sm => sm.Description)
                .HasMaxLength(500);

            builder.Property(sm => sm.BatchNumber)
                .HasMaxLength(50);

            builder.Property(sm => sm.SerialNumber)
                .HasMaxLength(100);

            // Foreign Keys - CASCADE DELETE KAPALI
            builder.HasOne(sm => sm.Tenant)
                .WithMany()
                .HasForeignKey(sm => sm.TenantId)
                .OnDelete(DeleteBehavior.Restrict); // NO ACTION

            builder.HasOne(sm => sm.Product)
                .WithMany(p => p.StockMovements)
                .HasForeignKey(sm => sm.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // NO ACTION

            builder.HasOne(sm => sm.Warehouse)
                .WithMany(w => w.StockMovements)
                .HasForeignKey(sm => sm.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict); // NO ACTION

            builder.HasOne(sm => sm.CreatedByUser)
                .WithMany(u => u.StockMovements)
                .HasForeignKey(sm => sm.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull) // User silinince null yap
                .IsRequired(false); // Nullable foreign key

            // Indexes
            builder.HasIndex(sm => new { sm.TenantId, sm.ProductId, sm.MovementDate });
            builder.HasIndex(sm => new { sm.TenantId, sm.MovementType });
            builder.HasIndex(sm => sm.Reference)
                .HasFilter("[Reference] IS NOT NULL");
        }
    }
}
