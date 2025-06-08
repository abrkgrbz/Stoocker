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
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs");

            builder.Property(al => al.Action)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(al => al.EntityName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(al => al.EntityId)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(al => al.IpAddress)
                .HasMaxLength(50);

            builder.Property(al => al.UserAgent)
                .HasMaxLength(500);

            // Foreign Keys
            builder.HasOne(al => al.Tenant)
                .WithMany()
                .HasForeignKey(al => al.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(al => al.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(al => al.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(al => new { al.TenantId, al.EntityName, al.Action });
            builder.HasIndex(al => new { al.TenantId, al.UserId });
            builder.HasIndex(al => al.CreatedAt);
        }
    }
}

