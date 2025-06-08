using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Stoocker.Domain.Entities;
using Stoocker.Domain.Enums;
using Stoocker.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Stoocker.Persistence.Seeds
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            try
            {
                // Database migration
                await context.Database.MigrateAsync();

                // Seed data
                await SeedDataAsync(context, userManager, roleManager);
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Database initialization error: {ex.Message}");
                throw;
            }
        }

        private static async Task SeedDataAsync(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            // 1. Demo Tenant oluştur
            if (!await context.Tenants.AnyAsync())
            {
                var demoTenant = new Tenant
                {
                    Id = Guid.NewGuid(),
                    Name = "Demo İşletme",
                    Domain = "demo",
                    IsActive = true,
                    PlanType = TenantPlan.Premium,
                    SubscriptionStartDate = DateTime.UtcNow,
                    SubscriptionEndDate = DateTime.UtcNow.AddYears(1),
                    ContactEmail = "demo@bizimdukkan.com",
                    ContactPhone = "+90 555 123 4567",
                    MaxUsers = 10,
                    MaxProducts = 5000,
                    MaxWarehouses = 3,
                    AllowMultiWarehouse = true,
                    AllowAdvancedReports = true
                };

                context.Tenants.Add(demoTenant);
                await context.SaveChangesAsync();

                // 2. Roller oluştur
                string[] roles = { "SuperAdmin", "Admin", "Manager", "User" };
                foreach (string roleName in roles)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        var role = new ApplicationRole
                        {
                            Name = roleName,
                            TenantId = demoTenant.Id,
                            Description = $"{roleName} role for demo tenant",
                            IsSystemRole = roleName == "SuperAdmin",
                            IsActive = true
                        };
                        await roleManager.CreateAsync(role);
                    }
                }

                // 3. Demo admin kullanıcı
                var adminEmail = "admin@demo.com";
                ApplicationUser adminUser = await userManager.FindByEmailAsync(adminEmail);

                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FirstName = "Admin",
                        LastName = "User",
                        TenantId = demoTenant.Id,
                        IsActive = true,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(adminUser, "Admin123!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }

                // 4. Temel birimler
                var units = new List<Unit>
                {
                    new Unit
                    {
                        TenantId = demoTenant.Id, Name = "Adet", ShortName = "Ad", AllowDecimals = false,
                        IsSystemUnit = true
                    },
                    new Unit
                    {
                        TenantId = demoTenant.Id, Name = "Kilogram", ShortName = "Kg", AllowDecimals = true,
                        IsSystemUnit = true
                    },
                    new Unit
                    {
                        TenantId = demoTenant.Id, Name = "Litre", ShortName = "Lt", AllowDecimals = true,
                        IsSystemUnit = true
                    },
                    new Unit
                    {
                        TenantId = demoTenant.Id, Name = "Metre", ShortName = "M", AllowDecimals = true,
                        IsSystemUnit = true
                    },
                    new Unit
                    {
                        TenantId = demoTenant.Id, Name = "Paket", ShortName = "Pkt", AllowDecimals = false,
                        IsSystemUnit = true
                    }
                };
                context.Units.AddRange(units);

                // 5. Demo kategoriler
                var categories = new List<Category>
                {
                    new Category
                    {
                        TenantId = demoTenant.Id,
                        Name = "Gıda",
                        Description = "Gıda ürünleri",
                        IsActive = true,
                        Color = "#e74c3c"
                    },
                    new Category
                    {
                        TenantId = demoTenant.Id,
                        Name = "İçecek",
                        Description = "İçecek ürünleri",
                        IsActive = true,
                        Color = "#3498db"
                    },
                    new Category
                    {
                        TenantId = demoTenant.Id,
                        Name = "Temizlik",
                        Description = "Temizlik ürünleri",
                        IsActive = true,
                        Color = "#2ecc71"
                    }
                };
                context.Categories.AddRange(categories);

                // 6. Demo markalar
                var brands = new List<Brand>
                {
                    new Brand { TenantId = demoTenant.Id, Name = "Coca Cola", IsActive = true },
                    new Brand { TenantId = demoTenant.Id, Name = "Ülker", IsActive = true },
                    new Brand { TenantId = demoTenant.Id, Name = "Ariel", IsActive = true },
                    new Brand { TenantId = demoTenant.Id, Name = "Nestle", IsActive = true }
                };
                context.Brands.AddRange(brands);

                // 7. Demo depo
                var warehouse = new Warehouse
                {
                    TenantId = demoTenant.Id,
                    Name = "Ana Depo",
                    Code = "ANA001",
                    Description = "Ana merkez deposu",
                    Address = "Demo Mahallesi, Demo Caddesi No:1",
                    City = "İstanbul",
                    Country = "Turkey",
                    IsDefault = true,
                    IsActive = true,
                    Type = WarehouseType.MainWarehouse
                };
                context.Warehouses.Add(warehouse);

                await context.SaveChangesAsync();

                // 8. Demo ürünler (kategoriler kaydedildikten sonra)
                var gidaCategory = categories.First(c => c.Name == "Gıda");
                var icecekCategory = categories.First(c => c.Name == "İçecek");
                var adBirim = units.First(u => u.ShortName == "Ad");
                var cocaColaBrand = brands.First(b => b.Name == "Coca Cola");
                var ulkerBrand = brands.First(b => b.Name == "Ülker");

                var products = new List<Product>
                {
                    new Product
                    {
                        TenantId = demoTenant.Id,
                        ProductCode = "COCA001",
                        Name = "Coca Cola 330ml",
                        Description = "Coca Cola kutu kola 330ml",
                        CategoryId = icecekCategory.Id,
                        BrandId = cocaColaBrand.Id,
                        UnitId = adBirim.Id,
                        PurchasePrice = 2.50m,
                        SalePrice = 4.00m,
                        MinimumStock = 50,
                        MaximumStock = 500,
                        IsActive = true,
                        Barcode = "8690548001234"
                    },
                    new Product
                    {
                        TenantId = demoTenant.Id,
                        ProductCode = "ULKER001",
                        Name = "Ülker Çikolata",
                        Description = "Ülker sütlü çikolata 80gr",
                        CategoryId = gidaCategory.Id,
                        BrandId = ulkerBrand.Id,
                        UnitId = adBirim.Id,
                        PurchasePrice = 3.00m,
                        SalePrice = 5.50m,
                        MinimumStock = 20,
                        MaximumStock = 200,
                        IsActive = true,
                        Barcode = "8690548005678"
                    }
                };
                context.Products.AddRange(products);
                await context.SaveChangesAsync();

                // 9. İlk stok kayıtları
                foreach (var product in products)
                {
                    var productStock = new ProductStock
                    {
                        ProductId = product.Id,
                        WarehouseId = warehouse.Id,
                        CurrentStock = 100, // Demo başlangıç stoğu
                        ReservedStock = 0,
                        AverageCost = product.PurchasePrice,
                        LastPurchasePrice = product.PurchasePrice,
                        LastMovementDate = DateTime.UtcNow
                    };
                    context.ProductStocks.Add(productStock);

                    // İlk stok hareketi
                    var stockMovement = new StockMovement
                    {
                        TenantId = demoTenant.Id,
                        ProductId = product.Id,
                        WarehouseId = warehouse.Id,
                        MovementType = StockMovementType.InitialStock,
                        Quantity = 100,
                        UnitPrice = product.PurchasePrice,
                        Description = "İlk stok girişi",
                        Reference = "INIT-001",
                        CreatedBy = adminUser?.Id
                    };
                    context.StockMovements.Add(stockMovement);
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
