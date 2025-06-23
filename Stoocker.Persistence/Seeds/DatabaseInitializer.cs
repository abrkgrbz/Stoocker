using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Stoocker.Domain.Constants;
using Stoocker.Domain.Entities;
using Stoocker.Domain.Enums;
using Stoocker.Persistence.Contexts;
using System.Reflection;

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

                // Seed permissions first
                await SeedPermissionsAsync(context);

                // Seed data with permissions
                await SeedDataAsync(context, userManager, roleManager);
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Database initialization error: {ex.Message}");
                throw;
            }
        }

        private static async Task SeedPermissionsAsync(ApplicationDbContext context)
        {
            if (await context.Permissions.AnyAsync())
                return;

            var permissionType = typeof(PermissionConstants);
            var fields = permissionType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string));

            var permissions = new List<Permission>();

            foreach (var field in fields)
            {
                var permissionName = field.GetValue(null)?.ToString();
                if (string.IsNullOrEmpty(permissionName))
                    continue;

                permissions.Add(new Permission
                {
                    Name = permissionName,
                    DisplayName = GetPermissionDisplayName(permissionName),
                    Category = GetPermissionCategory(permissionName),
                    Description = $"Permission for {GetPermissionDisplayName(permissionName)}",
                    IsActive = true,
                    SortOrder = permissions.Count
                });
            }

            context.Permissions.AddRange(permissions);
            await context.SaveChangesAsync();
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

                // 2. Roller oluştur ve permission ata
                await CreateRolesWithPermissionsAsync(context, roleManager, demoTenant.Id);

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

        private static async Task CreateRolesWithPermissionsAsync(
            ApplicationDbContext context,
            RoleManager<ApplicationRole> roleManager,
            Guid tenantId)
        {
            Console.WriteLine("Starting role and permission assignment...");

            // SuperAdmin rolü ve yetkileri
            var superAdminRole = await CreateRoleAsync(roleManager, "SuperAdmin", tenantId, "Süper Yönetici", true);
            if (superAdminRole != null)
            {
                Console.WriteLine($"SuperAdmin role created/found with ID: {superAdminRole.Id}");
                // SuperAdmin tüm yetkilere sahip
                var allPermissions = await context.Permissions.ToListAsync();
                Console.WriteLine($"Found {allPermissions.Count} total permissions");
                await AssignPermissionsToRoleAsync(context, superAdminRole.Id, allPermissions.Select(p => p.Id).ToArray());
            }

            // Admin rolü ve yetkileri
            var adminRole = await CreateRoleAsync(roleManager, "Admin", tenantId, "Yönetici", true);
            if (adminRole != null)
            {
                Console.WriteLine($"Admin role created/found with ID: {adminRole.Id}");
                var adminPermissions = new[]
                {
                    PermissionConstants.ProductView,
                    PermissionConstants.ProductCreate,
                    PermissionConstants.ProductUpdate,
                    PermissionConstants.ProductDelete,
                    PermissionConstants.ProductPriceUpdate,
                    PermissionConstants.ProductExport,
                    PermissionConstants.ProductImport,
                    PermissionConstants.CategoryView,
                    PermissionConstants.CategoryCreate,
                    PermissionConstants.CategoryUpdate,
                    PermissionConstants.CategoryDelete,
                    PermissionConstants.BrandView,
                    PermissionConstants.BrandCreate,
                    PermissionConstants.BrandUpdate,
                    PermissionConstants.BrandDelete,
                    PermissionConstants.CustomerView,
                    PermissionConstants.CustomerCreate,
                    PermissionConstants.CustomerUpdate,
                    PermissionConstants.CustomerDelete,
                    PermissionConstants.SupplierView,
                    PermissionConstants.SupplierCreate,
                    PermissionConstants.SupplierUpdate,
                    PermissionConstants.SupplierDelete,
                    PermissionConstants.SalesInvoiceView,
                    PermissionConstants.SalesInvoiceCreate,
                    PermissionConstants.SalesInvoiceUpdate,
                    PermissionConstants.SalesInvoiceApprove,
                    PermissionConstants.PurchaseInvoiceView,
                    PermissionConstants.PurchaseInvoiceCreate,
                    PermissionConstants.PurchaseInvoiceUpdate,
                    PermissionConstants.StockMovementView,
                    PermissionConstants.StockMovementCreate,
                    PermissionConstants.WarehouseView,
                    PermissionConstants.WarehouseCreate,
                    PermissionConstants.WarehouseUpdate,
                    PermissionConstants.ReportSalesView,
                    PermissionConstants.ReportStockView,
                    PermissionConstants.UserView,
                    PermissionConstants.UserCreate,
                    PermissionConstants.UserUpdate,
                    PermissionConstants.RoleView,
                    PermissionConstants.SettingsView,
                    PermissionConstants.SettingsUpdate
                };
                await AssignPermissionsByNameAsync(context, adminRole.Id, adminPermissions);
            }

            // Manager rolü ve yetkileri
            var managerRole = await CreateRoleAsync(roleManager, "Manager", tenantId, "Müdür", false);
            if (managerRole != null)
            {
                Console.WriteLine($"Manager role created/found with ID: {managerRole.Id}");
                var managerPermissions = new[]
                {
                    PermissionConstants.ProductView,
                    PermissionConstants.ProductCreate,
                    PermissionConstants.ProductUpdate,
                    PermissionConstants.ProductPriceUpdate,
                    PermissionConstants.CategoryView,
                    PermissionConstants.CustomerView,
                    PermissionConstants.CustomerCreate,
                    PermissionConstants.CustomerUpdate,
                    PermissionConstants.SupplierView,
                    PermissionConstants.SalesInvoiceView,
                    PermissionConstants.SalesInvoiceCreate,
                    PermissionConstants.SalesInvoiceUpdate,
                    PermissionConstants.PurchaseInvoiceView,
                    PermissionConstants.StockMovementView,
                    PermissionConstants.ReportSalesView,
                    PermissionConstants.ReportStockView
                };
                await AssignPermissionsByNameAsync(context, managerRole.Id, managerPermissions);
            }

            // User rolü ve yetkileri
            var userRole = await CreateRoleAsync(roleManager, "User", tenantId, "Kullanıcı", false);
            if (userRole != null)
            {
                Console.WriteLine($"User role created/found with ID: {userRole.Id}");
                var userPermissions = new[]
                {
                    PermissionConstants.ProductView,
                    PermissionConstants.CategoryView,
                    PermissionConstants.CustomerView,
                    PermissionConstants.SalesInvoiceView,
                    PermissionConstants.SalesInvoiceCreate,
                    PermissionConstants.ProductStockView
                };
                await AssignPermissionsByNameAsync(context, userRole.Id, userPermissions);
            }

            // Final check
            var rolePermissionCount = await context.RolePermissions.CountAsync();
            Console.WriteLine($"Total RolePermissions created: {rolePermissionCount}");
        }

        private static async Task<ApplicationRole?> CreateRoleAsync(
            RoleManager<ApplicationRole> roleManager,
            string roleName,
            Guid tenantId,
            string description,
            bool isSystemRole)
        {
            var existingRole = await roleManager.FindByNameAsync(roleName);
            if (existingRole != null)
            {
                return existingRole;
            }

            var role = new ApplicationRole
            {
                Id = Guid.NewGuid(),
                Name = roleName,
                NormalizedName = roleName.ToUpper(),
                TenantId = tenantId,
                Description = description,
                IsSystemRole = isSystemRole,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                return role;
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Role creation failed: {errors}");
            }
        }

        private static async Task AssignPermissionsToRoleAsync(
            ApplicationDbContext context,
            Guid roleId,
            Guid[] permissionIds)
        {
            foreach (var permissionId in permissionIds)
            {
                var rolePermission = new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permissionId,
                    IsGranted = true,
                    GrantedAt = DateTime.UtcNow
                };
                context.RolePermissions.Add(rolePermission);
            }
            await context.SaveChangesAsync();
        }

        private static async Task AssignPermissionsByNameAsync(
            ApplicationDbContext context,
            Guid roleId,
            string[] permissionNames)
        {
            var permissions = await context.Permissions
                .Where(p => permissionNames.Contains(p.Name))
                .ToListAsync();

            if (!permissions.Any())
            {
                Console.WriteLine($"Warning: No permissions found for names: {string.Join(", ", permissionNames)}");
                return;
            }

            foreach (var permission in permissions)
            {
                var existingRolePermission = await context.RolePermissions
                    .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permission.Id);

                if (existingRolePermission == null)
                {
                    var rolePermission = new RolePermission
                    {
                        Id = Guid.NewGuid(),
                        RoleId = roleId,
                        PermissionId = permission.Id,
                        IsGranted = true,
                        GrantedAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow
                    };
                    context.RolePermissions.Add(rolePermission);
                }
            }

            try
            {
                await context.SaveChangesAsync();
                Console.WriteLine($"Successfully assigned {permissions.Count} permissions to role {roleId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error assigning permissions: {ex.Message}");
                throw;
            }
        }

        private static string GetPermissionCategory(string permissionName)
        {
            var parts = permissionName.Split('.');
            return parts.Length > 0 ?
                char.ToUpper(parts[0][0]) + parts[0].Substring(1).ToLower() :
                "General";
        }

        private static string GetPermissionDisplayName(string permissionName)
        {
            return permissionName
                .Replace(".", " ")
                .Replace("_", " ")
                .Split(' ')
                .Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower())
                .Aggregate((current, next) => current + " " + next);
        }
    }
}