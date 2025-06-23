using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Domain.Constants
{
    public class PermissionConstants
    {
        // Product Permissions
        public const string ProductView = "product.view";
        public const string ProductCreate = "product.create";
        public const string ProductUpdate = "product.update";
        public const string ProductDelete = "product.delete";
        public const string ProductExport = "product.export";
        public const string ProductImport = "product.import";
        public const string ProductPriceUpdate = "product.price.update";
        public const string ProductStockView = "product.stock.view";
        public const string ProductStockUpdate = "product.stock.update";

        // Category Permissions
        public const string CategoryView = "category.view";
        public const string CategoryCreate = "category.create";
        public const string CategoryUpdate = "category.update";
        public const string CategoryDelete = "category.delete";

        // Brand Permissions
        public const string BrandView = "brand.view";
        public const string BrandCreate = "brand.create";
        public const string BrandUpdate = "brand.update";
        public const string BrandDelete = "brand.delete";

        // Customer Permissions
        public const string CustomerView = "customer.view";
        public const string CustomerCreate = "customer.create";
        public const string CustomerUpdate = "customer.update";
        public const string CustomerDelete = "customer.delete";
        public const string CustomerExport = "customer.export";
        public const string CustomerCreditView = "customer.credit.view";
        public const string CustomerCreditUpdate = "customer.credit.update";

        // Supplier Permissions
        public const string SupplierView = "supplier.view";
        public const string SupplierCreate = "supplier.create";
        public const string SupplierUpdate = "supplier.update";
        public const string SupplierDelete = "supplier.delete";

        // Sales Invoice Permissions
        public const string SalesInvoiceView = "sales.invoice.view";
        public const string SalesInvoiceCreate = "sales.invoice.create";
        public const string SalesInvoiceUpdate = "sales.invoice.update";
        public const string SalesInvoiceDelete = "sales.invoice.delete";
        public const string SalesInvoiceApprove = "sales.invoice.approve";
        public const string SalesInvoiceCancel = "sales.invoice.cancel";
        public const string SalesInvoicePrint = "sales.invoice.print";
        public const string SalesInvoiceExport = "sales.invoice.export";
        public const string SalesInvoiceDiscountApply = "sales.invoice.discount.apply";

        // Purchase Invoice Permissions
        public const string PurchaseInvoiceView = "purchase.invoice.view";
        public const string PurchaseInvoiceCreate = "purchase.invoice.create";
        public const string PurchaseInvoiceUpdate = "purchase.invoice.update";
        public const string PurchaseInvoiceDelete = "purchase.invoice.delete";
        public const string PurchaseInvoiceApprove = "purchase.invoice.approve";

        // Stock Movement Permissions
        public const string StockMovementView = "stock.movement.view";
        public const string StockMovementCreate = "stock.movement.create";
        public const string StockMovementAdjust = "stock.movement.adjust";
        public const string StockTransfer = "stock.transfer";
        public const string StockCount = "stock.count";

        // Warehouse Permissions
        public const string WarehouseView = "warehouse.view";
        public const string WarehouseCreate = "warehouse.create";
        public const string WarehouseUpdate = "warehouse.update";
        public const string WarehouseDelete = "warehouse.delete";
        public const string WarehouseManage = "warehouse.manage";

        // Report Permissions
        public const string ReportSalesView = "report.sales.view";
        public const string ReportPurchaseView = "report.purchase.view";
        public const string ReportStockView = "report.stock.view";
        public const string ReportFinancialView = "report.financial.view";
        public const string ReportExport = "report.export";

        // User Management Permissions
        public const string UserView = "user.view";
        public const string UserCreate = "user.create";
        public const string UserUpdate = "user.update";
        public const string UserDelete = "user.delete";
        public const string UserRoleAssign = "user.role.assign";

        // Role Management Permissions
        public const string RoleView = "role.view";
        public const string RoleCreate = "role.create";
        public const string RoleUpdate = "role.update";
        public const string RoleDelete = "role.delete";
        public const string RolePermissionManage = "role.permission.manage";

        // Tenant Management Permissions (Super Admin)
        public const string TenantView = "tenant.view";
        public const string TenantCreate = "tenant.create";
        public const string TenantUpdate = "tenant.update";
        public const string TenantDelete = "tenant.delete";
        public const string TenantPlanManage = "tenant.plan.manage";

        // Settings Permissions
        public const string SettingsView = "settings.view";
        public const string SettingsUpdate = "settings.update";
        public const string SettingsCompanyUpdate = "settings.company.update";

        // Audit Log Permissions
        public const string AuditLogView = "audit.log.view";
        public const string AuditLogExport = "audit.log.export";
    }
}
