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
    public class ApplicationUserRoleConfiguration : IEntityTypeConfiguration<ApplicationUserRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserRole> builder)
        {
            builder.ToTable("UserRoles");

            // Composite primary key
            builder.HasKey(ur => new { ur.UserId, ur.RoleId });

            // User relationship
            builder.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade); // User silinince UserRole da silinsin

            // Role relationship
            builder.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade); // Role silinince UserRole da silinsin

            // Additional properties
            builder.Property(ur => ur.AssignedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(ur => ur.IsActive)
                .HasDefaultValue(true);
        }
    }
}
