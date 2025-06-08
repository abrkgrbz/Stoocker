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
    public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
    {
        public void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            builder.ToTable("Warehouses");

            builder.Property(w => w.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(w => w.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(w => w.Description)
                .HasMaxLength(500);

            builder.Property(w => w.Address)
                .HasMaxLength(500);

            builder.Property(w => w.City)
                .HasMaxLength(100);

            builder.Property(w => w.Phone)
                .HasMaxLength(20);

            builder.Property(w => w.Email)
                .HasMaxLength(200);

            builder.Property(w => w.ManagerName)
                .HasMaxLength(200);

            // Decimal properties
            builder.Property(w => w.TotalArea).HasPrecision(18, 2);
            builder.Property(w => w.StorageCapacity).HasPrecision(18, 2);

            // Foreign Keys
            builder.HasOne(w => w.Tenant)
                .WithMany(t => t.Warehouses)
                .HasForeignKey(w => w.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(w => w.Manager)
                .WithMany()
                .HasForeignKey(w => w.ManagerUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            // Indexes
            builder.HasIndex(w => new { w.TenantId, w.Code })
                .IsUnique();

            builder.HasIndex(w => new { w.TenantId, w.IsDefault });
        }
    }
}
