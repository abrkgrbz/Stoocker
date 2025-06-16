using Stoocker.Application.DTOs.Common;
using Stoocker.Application.DTOs.User.Request;
using Stoocker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Application.DTOs.Tenant.Request;
using Stoocker.Application.DTOs.Tenant.Response;
using Stoocker.Application.DTOs.User.Response;

namespace Stoocker.Application.Interfaces.Services
{
    public interface ITenantService
    {     // Temel CRUD operasyonları
        Task<Result<TenantResponse>> GetByIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
        Task<Result<TenantResponse>> GetByDomainAsync(string domain, CancellationToken cancellationToken = default);
        Task<Result<PagedResult<TenantResponse>>> GetTenantsAsync(Guid tenantId,int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<Result<TenantDetailResponse>> GetTenantDetailsAsync(Guid tenantId, CancellationToken cancellationToken = default);
        Task<Result<TenantResponse>> CreateTenantAsync(CreateTenantRequest dto, CancellationToken cancellationToken = default);
        Task<Result> UpdateTenantAsync(UpdateTenantRequest dto, CancellationToken cancellationToken = default);
        Task<Result> DeleteTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);

        // Tenant durumu yönetimi
        Task<Result> ActivateTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
        Task<Result> DeactivateTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
        Task<Result> SuspendTenantAsync(Guid tenantId, string reason, CancellationToken cancellationToken = default);

        // Tenant istatistikleri
        Task<Result<TenantStatsResponse>> GetTenantStatsAsync(Guid tenantId, CancellationToken cancellationToken = default);
        Task<Result<int>> GetTenantUserCountAsync(Guid tenantId, CancellationToken cancellationToken = default);
        Task<Result<int>> GetTenantRoleCountAsync(Guid tenantId, CancellationToken cancellationToken = default);

        // Domain yönetimi
        Task<Result<bool>> IsDomainAvailableAsync(string domain, Guid? excludeTenantId = null, CancellationToken cancellationToken = default);
        Task<Result> UpdateTenantDomainAsync(Guid tenantId, string newDomain, CancellationToken cancellationToken = default);

        // Tenant ayarları
        Task<Result> UpdateTenantSettingsAsync(Guid tenantId, UpdateTenantSettingsRequest dto, CancellationToken cancellationToken = default);
        Task<Result<TenantSettingsResponse>> GetTenantSettingsAsync(Guid tenantId, CancellationToken cancellationToken = default);

        // Yardımcı metodlar
        Task<Result<bool>> TenantExistsAsync(Guid tenantId, CancellationToken cancellationToken = default);
        Task<Result<bool>> CanDeleteTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
        Task<Result<List<TenantResponse>>> GetActiveTenantsAsync(CancellationToken cancellationToken = default);

        // Tenant initialization
        Task<Result> InitializeTenantAsync(Guid tenantId, InitializeTenantRequest dto, CancellationToken cancellationToken = default);


    }
}
