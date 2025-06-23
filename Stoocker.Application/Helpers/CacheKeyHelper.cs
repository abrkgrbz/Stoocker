using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.Helpers
{
    public static class CacheKeyHelper
    {
        private const string Prefix = "Stoocker";

        //Brand cache keys
        public static string BrandById(Guid brandId) => $"{Prefix}:Brand:Id:{brandId}";

        // User cache keys
        public static string UserById(Guid userId) => $"{Prefix}:User:Id:{userId}";
        public static string UserByEmail(string email) => $"{Prefix}:User:Email:{email}";
        public static string UserRoles(Guid userId) => $"{Prefix}:User:Roles:{userId}";

        // Tenant cache keys
        public static string TenantById(Guid tenantId) => $"{Prefix}:Tenant:Id:{tenantId}";
        public static string TenantByCode(string code) => $"{Prefix}:Tenant:Code:{code}";
        public static string TenantUsers(Guid tenantId) => $"{Prefix}:Tenant:Users:{tenantId}";

        // Product cache keys
        public static string ProductById(Guid productId, Guid tenantId) => $"{Prefix}:Tenant:{tenantId}:Product:Id:{productId}";
        public static string ProductList(Guid tenantId, int page, int size) => $"{Prefix}:Tenant:{tenantId}:Products:Page:{page}:Size:{size}";
        public static string ProductByBarcode(string barcode, Guid tenantId) => $"{Prefix}:Tenant:{tenantId}:Product:Barcode:{barcode}";

        // Category cache keys
        public static string CategoryById(Guid categoryId, Guid tenantId) => $"{Prefix}:Tenant:{tenantId}:Category:Id:{categoryId}";
        public static string CategoryList(Guid tenantId) => $"{Prefix}:Tenant:{tenantId}:Categories";

        // Stock cache keys
        public static string StockByProduct(Guid productId, Guid warehouseId, Guid tenantId) =>
            $"{Prefix}:Tenant:{tenantId}:Stock:Product:{productId}:Warehouse:{warehouseId}";

        // Pattern helpers
        public static string TenantPattern(Guid tenantId) => $"{Prefix}:Tenant:{tenantId}:*";
        public static string UserPattern(Guid userId) => $"{Prefix}:User:*:{userId}*";
        public static string ProductPattern(Guid tenantId) => $"{Prefix}:Tenant:{tenantId}:Product*";
    }
}
