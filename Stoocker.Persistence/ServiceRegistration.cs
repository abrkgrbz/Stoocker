using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Stoocker.Application.Interfaces.Repositories;
using Stoocker.Application.Interfaces.Repositories.Specification;
using Stoocker.Domain.Entities;
using Stoocker.Persistence.Contexts;
using Stoocker.Persistence.Repositories;

namespace Stoocker.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceServices(this IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                     options.UseSqlServer(
                         Configuration.ConnectionString,
                         b => b.MigrationsAssembly("Stoocker.Persistence"))
             );

            services.AddIdentityCore<ApplicationUser>(options =>
                 {
                     // Password settings
                     options.Password.RequireDigit = true;
                     options.Password.RequireLowercase = true;
                     options.Password.RequireNonAlphanumeric = false;
                     options.Password.RequireUppercase = true;
                     options.Password.RequiredLength = 6;
                     options.Password.RequiredUniqueChars = 1;

                     // Lockout settings
                     options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                     options.Lockout.MaxFailedAccessAttempts = 5;
                     options.Lockout.AllowedForNewUsers = true;

                     // User settings
                     options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                     options.User.RequireUniqueEmail = true;

                     // Email confirmation
                     options.SignIn.RequireConfirmedEmail = false; // Development için false
                     options.SignIn.RequireConfirmedAccount = false;
                 })
                 .AddRoles<ApplicationRole>()
                 .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
            services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));
            services.AddScoped(typeof(ISpecificationRepository<>), typeof(SpecificationRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
