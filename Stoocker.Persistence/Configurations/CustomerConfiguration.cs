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
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            builder.Property(c => c.CustomerCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Email)
                .HasMaxLength(200);

            builder.Property(c => c.Phone)
                .HasMaxLength(20);

            builder.Property(c => c.TaxNumber)
                .HasMaxLength(50);

            builder.Property(c => c.CreditLimit)
                .HasPrecision(18, 2);

            builder.Property(c => c.CurrentBalance)
                .HasPrecision(18, 2);

            builder.Property(c => c.TotalSales)
                .HasPrecision(18, 2);

            // Tenant relationship
            builder.HasOne(c => c.Tenant)
                .WithMany(t => t.Customers)
                .HasForeignKey(c => c.TenantId)
                .OnDelete(DeleteBehavior.Restrict); // NO ACTION

            // Indexes
            builder.HasIndex(c => new { c.TenantId, c.CustomerCode })
                .IsUnique();

            builder.HasIndex(c => c.TaxNumber)
                .HasFilter("[TaxNumber] IS NOT NULL");
        }
    }
}
