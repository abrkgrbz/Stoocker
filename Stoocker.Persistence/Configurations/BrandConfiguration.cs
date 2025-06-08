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
    public class BrandConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.ToTable("Brands");

            builder.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(b => b.Description)
                .HasMaxLength(500);

            builder.Property(b => b.Website)
                .HasMaxLength(300);

            builder.Property(b => b.ContactEmail)
                .HasMaxLength(200);

            builder.Property(b => b.ContactPhone)
                .HasMaxLength(20);

            // Tenant relationship
            builder.HasOne(b => b.Tenant)
                .WithMany(t => t.Brands)
                .HasForeignKey(b => b.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(b => new { b.TenantId, b.Name });
        }
    }
}
