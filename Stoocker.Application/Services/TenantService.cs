using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Stoocker.Application.DTOs.Common;
using Stoocker.Application.DTOs.Tenant.Request;
using Stoocker.Application.DTOs.Tenant.Response;
using Stoocker.Application.Interfaces.Repositories;
using Stoocker.Application.Interfaces.Repositories.Entities.Tenant;
using Stoocker.Application.Interfaces.Services;
using Stoocker.Domain.Entities;
using TenantStatus = Stoocker.Application.DTOs.Tenant.Response.TenantStatus;

namespace Stoocker.Application.Services
{
    public class TenantService:ITenantService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TenantService> _logger;
        private readonly IMapper _mapper; 

        public TenantService(IUnitOfWork unitOfWork, ILogger<TenantService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<TenantResponse>> GetByIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            try
            {
                var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId, cancellationToken);
                if (tenant == null)
                {
                    return Result<TenantResponse>.Failure("Tenant bulunamadı.");
                }

                var response = _mapper.Map<TenantResponse>(tenant);
                return Result<TenantResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tenant getirme sırasında hata oluştu. TenantId: {TenantId}", tenantId);
                return Result<TenantResponse>.Failure("Tenant bilgileri alınırken bir hata oluştu.");
            }
        }

        public async Task<Result<TenantResponse>> GetByDomainAsync(string domain, CancellationToken cancellationToken = default)
        {
            try
            {

                var tenant = await _unitOfWork.Tenants.FirstOrDefaultAsync(t => t.Domain == domain, cancellationToken);
                if (tenant == null)
                {
                    return Result<TenantResponse>.Failure("Belirtilen domain için tenant bulunamadı.");
                }

                var response = _mapper.Map<TenantResponse>(tenant);
                return Result<TenantResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Domain ile tenant getirme sırasında hata oluştu. Domain: {Domain}", domain);
                return Result<TenantResponse>.Failure("Tenant bilgileri alınırken bir hata oluştu.");
            }
        }

        public async Task<Result<PagedResult<TenantResponse>>> GetTenantsAsync(Guid tenantId,int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            try
            { 
                var tenants = await _unitOfWork.TenantSpecs.GetPagedAsync(
                    tenantId: tenantId,
                    page: page,
                    pageSize: pageSize, 
                    cancellationToken: cancellationToken);

                var responses = _mapper.Map<PagedResult<TenantResponse>>(tenants);
                return Result<PagedResult<TenantResponse>>.Success(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tenant listesi getirme sırasında hata oluştu. Page: {Page}, PageSize: {PageSize}", page, pageSize);
                return Result<PagedResult<TenantResponse>>.Failure("Tenant listesi alınırken bir hata oluştu.");
            }
        }

        public async Task<Result<TenantDetailResponse>> GetTenantDetailsAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            try
            {
                var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId, cancellationToken);
                if (tenant == null)
                {
                    return Result<TenantDetailResponse>.Failure("Tenant bulunamadı.");
                }

                // User count
                var userCount = await _unitOfWork.Users.CountAsync(u => u.TenantId == tenantId, cancellationToken);
                var activeUserCount = await _unitOfWork.Users.CountAsync(u => u.TenantId == tenantId && u.IsActive, cancellationToken);

                // Role count
                var roleCount = await _unitOfWork.Roles.CountAsync(r => r.TenantId == tenantId, cancellationToken);

                var response = _mapper.Map<TenantDetailResponse>(tenant);
                response.UserCount = userCount;
                response.ActiveUserCount = activeUserCount;
                response.RoleCount = roleCount;

                return Result<TenantDetailResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tenant detay getirme sırasında hata oluştu. TenantId: {TenantId}", tenantId);
                return Result<TenantDetailResponse>.Failure("Tenant detayları alınırken bir hata oluştu.");
            }
        }

        public async Task<Result<TenantResponse>> CreateTenantAsync(CreateTenantRequest dto, CancellationToken cancellationToken = default)
        {
            try
            {
                // Domain uniqueness check
                if (!string.IsNullOrEmpty(dto.Domain))
                {
                    var domainExists = await _unitOfWork.Tenants.AnyAsync(t => t.Domain == dto.Domain, cancellationToken);
                    if (domainExists)
                    {
                        return Result<TenantResponse>.Failure("Bu domain zaten kullanımda.");
                    }
                }

                var tenant = new Tenant
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    Domain = dto.Domain,
                    ContactEmail = dto.ContactEmail,
                    ContactPhone = dto.ContactPhone,
                    PrimaryColor = dto.PrimaryColor,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.GetWriteRepository<Tenant>().AddAsync(tenant, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Yeni tenant oluşturuldu. TenantId: {TenantId}, Name: {Name}", tenant.Id, tenant.Name);

                var response = _mapper.Map<TenantResponse>(tenant);
                return Result<TenantResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tenant oluşturma sırasında hata oluştu. Name: {Name}", dto.Name);
                return Result<TenantResponse>.Failure("Tenant oluşturulurken bir hata oluştu.");
            }
        }

        public async Task<Result> UpdateTenantAsync(UpdateTenantRequest dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var tenant = await _unitOfWork.Tenants.GetByIdAsync(dto.Id, cancellationToken);
                if (tenant == null)
                {
                    return Result.Failure("Tenant bulunamadı.");
                }

                tenant.Name = dto.Name;
                tenant.ContactEmail = dto.ContactEmail;
                tenant.ContactPhone = dto.ContactPhone;
                tenant.PrimaryColor = dto.PrimaryColor;
                tenant.IsActive = dto.IsActive;
                tenant.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.GetWriteRepository<Tenant>().Update(tenant,cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Tenant güncellendi. TenantId: {TenantId}", dto.Id);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tenant güncelleme sırasında hata oluştu. TenantId: {TenantId}", dto.Id);
                return Result.Failure("Tenant güncellenirken bir hata oluştu.");
            }
        }

        public async Task<Result> DeleteTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            try
            {
                var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId, cancellationToken);
                if (tenant == null)
                {
                    return Result.Failure("Tenant bulunamadı.");
                }

                // Check if tenant can be deleted
                var canDelete = await CanDeleteTenantAsync(tenantId, cancellationToken);
                if (!canDelete.Data)
                {
                    return Result.Failure("Bu tenant'ta aktif kullanıcılar bulunduğu için silinemez.");
                }

                _unitOfWork.GetWriteRepository<Tenant>().Remove(tenant, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Tenant silindi. TenantId: {TenantId}", tenantId);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tenant silme sırasında hata oluştu. TenantId: {TenantId}", tenantId);
                return Result.Failure("Tenant silinirken bir hata oluştu.");
            }
        }

        public async Task<Result> ActivateTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            try
            {
                var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId, cancellationToken);
                if (tenant == null)
                {
                    return Result.Failure("Tenant bulunamadı.");
                }

                tenant.IsActive = true;
                tenant.UpdatedAt = DateTime.UtcNow;

                 _unitOfWork.GetWriteRepository<Tenant>().Update(tenant, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Tenant aktifleştirildi. TenantId: {TenantId}", tenantId);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tenant aktifleştirme sırasında hata oluştu. TenantId: {TenantId}", tenantId);
                return Result.Failure("Tenant aktifleştirilirken bir hata oluştu.");
            }
        }

        public async Task<Result> DeactivateTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            try
            {
                var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId, cancellationToken);
                if (tenant == null)
                {
                    return Result.Failure("Tenant bulunamadı.");
                }

                tenant.IsActive = false;
                tenant.UpdatedAt = DateTime.UtcNow;

                 _unitOfWork.GetWriteRepository<Tenant>().Update(tenant, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Tenant pasifleştirildi. TenantId: {TenantId}", tenantId);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tenant pasifleştirme sırasında hata oluştu. TenantId: {TenantId}", tenantId);
                return Result.Failure("Tenant pasifleştirilirken bir hata oluştu.");
            }
        }

        public async Task<Result> SuspendTenantAsync(Guid tenantId, string reason, CancellationToken cancellationToken = default)
        {
            try
            {
                var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId, cancellationToken);
                if (tenant == null)
                {
                    return Result.Failure("Tenant bulunamadı.");
                }

                tenant.IsActive = false;
                tenant.UpdatedAt = DateTime.UtcNow;
                // Note: Suspension reason should be stored in a separate entity or field

                 _unitOfWork.GetWriteRepository<Tenant>().Update(tenant, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Tenant askıya alındı. TenantId: {TenantId}, Reason: {Reason}", tenantId, reason);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tenant askıya alma sırasında hata oluştu. TenantId: {TenantId}", tenantId);
                return Result.Failure("Tenant askıya alınırken bir hata oluştu.");
            }
        }

        public async Task<Result<TenantStatsResponse>> GetTenantStatsAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            try
            {
                var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId, cancellationToken);
                if (tenant == null)
                {
                    return Result<TenantStatsResponse>.Failure("Tenant bulunamadı.");
                } 
                var stats = _mapper.Map<TenantStatsResponse>(tenant);
                 
                var totalUsers = await _unitOfWork.Users.CountAsync(u => u.TenantId == tenantId, cancellationToken);
                var activeUsers = await _unitOfWork.Users.CountAsync(u => u.TenantId == tenantId && u.IsActive, cancellationToken);
                var totalRoles = await _unitOfWork.Roles.CountAsync(r => r.TenantId == tenantId, cancellationToken);
                 
                stats.TotalUsers = totalUsers;
                stats.ActiveUsers = activeUsers;
                stats.InactiveUsers = totalUsers - activeUsers;
                stats.TotalRoles = totalRoles;
                stats.ActiveRoles = totalRoles; // Assuming all roles are active for now

                return Result<TenantStatsResponse>.Success(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tenant istatistikleri alınırken hata oluştu. TenantId: {TenantId}", tenantId);
                return Result<TenantStatsResponse>.Failure("Tenant istatistikleri alınırken bir hata oluştu.");
            }
        }

        public async Task<Result<int>> GetTenantUserCountAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            try
            {
                var count = await _unitOfWork.Users.CountAsync(u => u.TenantId == tenantId, cancellationToken);
                return Result<int>.Success(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tenant kullanıcı sayısı alınırken hata oluştu. TenantId: {TenantId}", tenantId);
                return Result<int>.Failure("Kullanıcı sayısı alınırken bir hata oluştu.");
            }
        }

        public async Task<Result<int>> GetTenantRoleCountAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            try
            {
                var count = await _unitOfWork.Roles.CountAsync(r => r.TenantId == tenantId, cancellationToken);
                return Result<int>.Success(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tenant rol sayısı alınırken hata oluştu. TenantId: {TenantId}", tenantId);
                return Result<int>.Failure("Rol sayısı alınırken bir hata oluştu.");
            }
        }

        public async Task<Result<bool>> IsDomainAvailableAsync(string domain, Guid? excludeTenantId = null, CancellationToken cancellationToken = default)
        {
            try
            {
                Expression<Func<Tenant, bool>> predicate = t => t.Domain == domain;

                if (excludeTenantId.HasValue)
                {
                    predicate = t => t.Domain == domain && t.Id != excludeTenantId.Value;
                }

                var exists = await _unitOfWork.Tenants.AnyAsync(predicate, cancellationToken);
                return Result<bool>.Success(!exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Domain müsaitlik kontrolü sırasında hata oluştu. Domain: {Domain}", domain);
                return Result<bool>.Failure("Domain kontrolü yapılırken bir hata oluştu.");
            }
        }

        public async Task<Result> UpdateTenantDomainAsync(Guid tenantId, string newDomain, CancellationToken cancellationToken = default)
        {
            try
            {
                var isAvailable = await IsDomainAvailableAsync(newDomain, tenantId, cancellationToken);
                if (!isAvailable.IsSuccess || !isAvailable.Data)
                {
                    return Result.Failure("Bu domain zaten kullanımda.");
                }

                var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId, cancellationToken);
                if (tenant == null)
                {
                    return Result.Failure("Tenant bulunamadı.");
                }

                tenant.Domain = newDomain;
                tenant.UpdatedAt = DateTime.UtcNow;

                 _unitOfWork.GetWriteRepository<Tenant>().Update(tenant, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Tenant domain güncellendi. TenantId: {TenantId}, NewDomain: {NewDomain}", tenantId, newDomain);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tenant domain güncelleme sırasında hata oluştu. TenantId: {TenantId}", tenantId);
                return Result.Failure("Domain güncellenirken bir hata oluştu.");
            }
        }

        public async Task<Result> UpdateTenantSettingsAsync(Guid tenantId, UpdateTenantSettingsRequest dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId, cancellationToken);
                if (tenant == null)
                {
                    return Result.Failure("Tenant bulunamadı.");
                }

                // Note: Tenant settings should be stored in a separate entity
                // For now, we'll just update the primary color
                tenant.PrimaryColor = dto.PrimaryColor;
                tenant.UpdatedAt = DateTime.UtcNow;

                 _unitOfWork.GetWriteRepository<Tenant>().Update(tenant, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Tenant ayarları güncellendi. TenantId: {TenantId}", tenantId);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tenant ayarları güncelleme sırasında hata oluştu. TenantId: {TenantId}", tenantId);
                return Result.Failure("Tenant ayarları güncellenirken bir hata oluştu.");
            }
        }

        public async Task<Result<TenantSettingsResponse>> GetTenantSettingsAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            try
            {
                var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId, cancellationToken);
                if (tenant == null)
                {
                    return Result<TenantSettingsResponse>.Failure("Tenant bulunamadı.");
                }

                var response = _mapper.Map<TenantSettingsResponse>(tenant);

                return Result<TenantSettingsResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tenant ayarları alınırken hata oluştu. TenantId: {TenantId}", tenantId);
                return Result<TenantSettingsResponse>.Failure("Tenant ayarları alınırken bir hata oluştu.");
            }
        }

        public async Task<Result<bool>> TenantExistsAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            try
            {
                var exists = await _unitOfWork.Tenants.AnyAsync(t => t.Id == tenantId, cancellationToken);
                return Result<bool>.Success(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tenant varlık kontrolü sırasında hata oluştu. TenantId: {TenantId}", tenantId);
                return Result<bool>.Failure("Tenant kontrolü yapılırken bir hata oluştu.");
            }
        }

        public async Task<Result<bool>> CanDeleteTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            try
            {
                var userCount = await _unitOfWork.Users.CountAsync(u => u.TenantId == tenantId, cancellationToken);
                return Result<bool>.Success(userCount == 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tenant silme kontrolü sırasında hata oluştu. TenantId: {TenantId}", tenantId);
                return Result<bool>.Failure("Silme kontrolü yapılırken bir hata oluştu.");
            }
        }

        public async Task<Result<List<TenantResponse>>> GetActiveTenantsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var tenants = await _unitOfWork.Tenants.FindAsync(t=>t.IsActive,cancellationToken);
                var responses = _mapper.Map<List<TenantResponse>>(tenants);
                return Result<List<TenantResponse>>.Success(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aktif tenant listesi alınırken hata oluştu");
                return Result<List<TenantResponse>>.Failure("Aktif tenant listesi alınırken bir hata oluştu.");
            }
        }

        public async Task<Result> InitializeTenantAsync(Guid tenantId, InitializeTenantRequest dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var tenant = await _unitOfWork.Tenants.GetByIdAsync(tenantId, cancellationToken);
                if (tenant == null)
                {
                    return Result.Failure("Tenant bulunamadı.");
                }

                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                try
                {
                    // Create default roles if requested
                    if (dto.DefaultData.CreateDefaultRoles)
                    {
                        foreach (var roleSetup in dto.DefaultData.Roles)
                        {
                            var role = new ApplicationRole
                            {
                                Id = Guid.NewGuid(),
                                Name = roleSetup.Name,
                                NormalizedName = roleSetup.Name.ToUpperInvariant(),
                                Description = roleSetup.Description,
                                TenantId = tenantId,
                                CreatedAt = DateTime.UtcNow
                            };

                            await _unitOfWork.Roles.AddAsync(role, cancellationToken);
                            await _unitOfWork.SaveChangesAsync(cancellationToken);
                        }
                    }

                    await _unitOfWork.CommitTransactionAsync(cancellationToken);

                    _logger.LogInformation("Tenant başlatıldı. TenantId: {TenantId}", tenantId);
                    return Result.Success();
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tenant başlatma sırasında hata oluştu. TenantId: {TenantId}", tenantId);
                return Result.Failure("Tenant başlatılırken bir hata oluştu.");
            }
        }
         
    }
}
