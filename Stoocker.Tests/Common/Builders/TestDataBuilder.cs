using Bogus;
using Stoocker.Domain.Entities;
using Stoocker.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Tests.Common.Builders
{
    public static class TestDataBuilder
    {
        private static readonly Faker Faker = new("tr");

        public static class TenantBuilder
        {
            public static Tenant Create(Action<Tenant>? customize = null)
            {
                var tenant = new Tenant
                {
                    Id = Guid.NewGuid(),
                    Name = Faker.Company.CompanyName(),
                    Domain = Faker.Internet.DomainWord(),
                    IsActive = true,
                    PlanType = TenantPlan.Premium,
                    MaxUsers = 10,
                    MaxProducts = 1000,
                    MaxWarehouses = 3,
                    AllowMultiWarehouse = true,
                    AllowAdvancedReports = true,
                    ContactEmail = Faker.Internet.Email(),
                    ContactPhone = Faker.Phone.PhoneNumber(),
                    CreatedAt = DateTime.UtcNow
                };

                customize?.Invoke(tenant);
                return tenant;
            }
        }

        public static class UserBuilder
        {
            public static ApplicationUser Create(Guid tenantId, Action<ApplicationUser>? customize = null)
            {
                var user = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    UserName = Faker.Internet.Email(),
                    Email = Faker.Internet.Email(),
                    NormalizedEmail = Faker.Internet.Email().ToUpperInvariant(),
                    NormalizedUserName = Faker.Internet.Email().ToUpperInvariant(),
                    FirstName = Faker.Name.FirstName(),
                    LastName = Faker.Name.LastName(),
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                };

                customize?.Invoke(user);
                return user;
            }

            public static List<ApplicationUser> CreateMany(Guid tenantId, int count = 3)
            {
                var users = new List<ApplicationUser>();
                for (int i = 0; i < count; i++)
                {
                    users.Add(Create(tenantId));
                }

                return users;
            }
        }

        public static class RoleBuilder
        {
            public static ApplicationRole Create(Guid tenantId, Action<ApplicationRole>? customize = null)
            {
                var role = new ApplicationRole
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Name = Faker.Name.JobTitle(),
                    NormalizedName = Faker.Name.JobTitle().ToUpperInvariant(),
                    Description = Faker.Lorem.Sentence(),
                    IsActive = true,
                    IsSystemRole = false,
                    CreatedAt = DateTime.UtcNow,
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                };

                customize?.Invoke(role);
                return role;
            }

            public static ApplicationRole CreateAdminRole(Guid tenantId)
            {
                return Create(tenantId, role =>
                {
                    role.Name = "Admin";
                    role.NormalizedName = "ADMIN";
                    role.Description = "Administrator role";
                    role.IsSystemRole = true;
                });
            }

            public static ApplicationRole CreateUserRole(Guid tenantId)
            {
                return Create(tenantId, role =>
                {
                    role.Name = "User";
                    role.NormalizedName = "USER";
                    role.Description = "Standard user role";
                });
            }
        }

    }
}
