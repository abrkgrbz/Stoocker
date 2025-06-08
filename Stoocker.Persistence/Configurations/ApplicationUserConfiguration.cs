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
    public class ApplicationUserConfiguration: IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("Users");

            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.TimeZone)
                .HasMaxLength(50)
                .HasDefaultValue("Europe/Istanbul");

            builder.Property(u => u.Language)
                .HasMaxLength(10)
                .HasDefaultValue("tr-TR");

            // Tenant ilişkisi - CASCADE DELETE KAPALI
            builder.HasOne(u => u.Tenant)
                .WithMany(t => t.Users)
                .HasForeignKey(u => u.TenantId)
                .OnDelete(DeleteBehavior.Restrict); // NO ACTION

            // Index
            builder.HasIndex(u => new { u.TenantId, u.Email })
                .IsUnique();
        }
    }
}
