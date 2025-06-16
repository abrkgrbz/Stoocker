using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Stoocker.Application.Interfaces.Repositories;
using Stoocker.Application.Interfaces.Repositories.Specification;
using Stoocker.Domain.Entities;
using Stoocker.Persistence.Contexts;
using Stoocker.Persistence.Repositories;
using Stoocker.Application.Interfaces.Repositories.Entities.ApplicationRole;
using Stoocker.Application.Interfaces.Repositories.Entities.ApplicationUser;
using Stoocker.Application.Interfaces.Repositories.Entities.ApplicationUserRole;
using Stoocker.Application.Interfaces.Repositories.Entities.Brand;
using Stoocker.Application.Interfaces.Repositories.Entities.Category;
using Stoocker.Application.Interfaces.Repositories.Entities.Customer;
using Stoocker.Application.Interfaces.Repositories.Entities.Product;
using Stoocker.Application.Interfaces.Repositories.Entities.ProductStock;
using Stoocker.Application.Interfaces.Repositories.Entities.PurchaseInvoice;
using Stoocker.Application.Interfaces.Repositories.Entities.PurchaseInvoiceDetail;
using Stoocker.Application.Interfaces.Repositories.Entities.SalesInvoice;
using Stoocker.Application.Interfaces.Repositories.Entities.StockMovement;
using Stoocker.Application.Interfaces.Repositories.Entities.Supplier;
using Stoocker.Application.Interfaces.Repositories.Entities.Tenant;
using Stoocker.Application.Interfaces.Repositories.Entities.Unit;
using Stoocker.Application.Interfaces.Repositories.Entities.Warehouse;
using Stoocker.Persistence.Repositories.Entities.Tenant;
using Stoocker.Persistence.Repositories.Entities;
using Stoocker.Persistence.Repositories.Entities.Brand;
using Stoocker.Persistence.Repositories.Entities.Category;
using Stoocker.Persistence.Repositories.Entities.Customer;
using Stoocker.Persistence.Repositories.Entities.Product;
using Stoocker.Persistence.Repositories.Entities.ProductStock;
using Stoocker.Persistence.Repositories.Entities.PurchaseInvoice;
using Stoocker.Persistence.Repositories.Entities.PurchaseInvoiceDetail;
using Stoocker.Persistence.Repositories.Entities.SalesInvoice;
using Stoocker.Persistence.Repositories.Entities.StockMovement;
using Stoocker.Persistence.Repositories.Entities.Supplier;
using Stoocker.Persistence.Repositories.Entities.Tenant.Specifications;
using Stoocker.Persistence.Repositories.Entities.Unit;
using Stoocker.Persistence.Repositories.Entities.Warehouse;
using Stoocker.Persistence.Repositories.Specification;

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

            // JWT Authentication
            services.AddAuthentication(options =>
                 {
                     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                 })
                 .AddJwtBearer(options =>
                 {
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuerSigningKey = true,
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetJwtSettings().SecretKey)),
                         ValidateIssuer = true,
                         ValidIssuer = Configuration.GetJwtSettings().Issuer,
                         ValidateAudience = true,
                         ValidAudience = Configuration.GetJwtSettings().Audience,
                         ClockSkew = TimeSpan.Zero
                     };
                 });
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Generic Repositories
            services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
            services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));
            services.AddScoped(typeof(ISpecificationRepository<>), typeof(SpecificationRepository<>));

            services.AddScoped(typeof(ITenantReadRepository<>), typeof(TenantReadRepository<>));
            services.AddScoped(typeof(ITenantWriteRepository<>), typeof(TenantWriteRepository<>));
            services.AddScoped<ITenantSpecReadRepository, TenantSpecReadRepository>();

            // Specific Repositories
            services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
            services.AddScoped<IApplicationRoleRepository, ApplicationRoleRepository>();
            services.AddScoped<IApplicationUserRoleRepository, ApplicationUserRoleRepository>();

            services.AddScoped<IBrandReadRepository, BrandReadRepository>();
            services.AddScoped<IBrandWriteRepository, BrandWriteRepository>();

            services.AddScoped<ICategoryReadRepository, CategoryReadRepository>();
            services.AddScoped<ICategoryWriteRepository, CategoryWriteRepository>();

            services.AddScoped<ICustomerReadRepository, CustomerReadRepository>();
            services.AddScoped<ICustomerWriteRepository, CustomerWriteRepository>();

            services.AddScoped<IProductReadRepository, ProductReadRepository>();
            services.AddScoped<IProductWriteRepository, ProductWriteRepository>();

            services.AddScoped<IProductStockReadRepository, ProductStockReadRepository>();
            services.AddScoped<IProductStockWriteRepository, ProductStockWriteRepository>();

            services.AddScoped<IPurchaseInvoiceReadRepository, PurchaseInvoiceReadRepository>();
            services.AddScoped<IPurchaseInvoiceWriteRepository, PurchaseInvoiceWriteRepository>();

            services.AddScoped<IPurchaseInvoiceDetailReadRepository, PurchaseInvoiceDetailReadRepository>();
            services.AddScoped<IPurchaseInvoiceDetailWriteRepository, PurchaseInvoiceDetailWriteRepository>();

            services.AddScoped<ISalesInvoiceReadRepository, SalesInvoiceReadRepository>();
            services.AddScoped<ISalesInvoiceWriteRepository, SalesInvoiceWriteRepository>();

            services.AddScoped<IStockMovementReadRepository, StockMovementReadRepository>();
            services.AddScoped<IStockMovementWriteRepository, StockMovementWriteRepository>();

            services.AddScoped<ISupplierReadRepository, SupplierReadRepository>();
            services.AddScoped<ISupplierWriteRepository, SupplierWriteRepository>();

            services.AddScoped<IUnitReadRepository, UnitReadRepository>();
            services.AddScoped<IUnitWriteRepository, UnitWriteRepository>();

            services.AddScoped<IWarehouseReadRepository, WarehouseReadRepository>();
            services.AddScoped<IWarehouseWriteRepository, WarehouseWriteRepository>();
        }
    }
}
