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
    public class TenantConfiguration: IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.ToTable("Tenants");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Domain)
                .HasMaxLength(100);

            builder.Property(t => t.ContactEmail)
                .HasMaxLength(200);

            builder.Property(t => t.ContactPhone)
                .HasMaxLength(20);

            builder.Property(t => t.PrimaryColor)
                .HasMaxLength(7)
                .HasDefaultValue("#3498db");

            // Index
            builder.HasIndex(t => t.Domain)
                .IsUnique()
                .HasFilter("[Domain] IS NOT NULL");
        }
    }
}
