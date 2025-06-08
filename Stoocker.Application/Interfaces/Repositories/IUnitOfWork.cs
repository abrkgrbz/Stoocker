using Stoocker.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Application.Interfaces.Repositories.Entities.ApplicationRole;
using Stoocker.Application.Interfaces.Repositories.Entities.ApplicationUser;
using Stoocker.Application.Interfaces.Repositories.Entities.ApplicationUserRole;
using Stoocker.Application.Interfaces.Repositories.Entities.Tenant;
using Stoocker.Domain.Entities;

namespace Stoocker.Application.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
// Generic Repositories
        IReadRepository<T> GetReadRepository<T>() where T : BaseEntity;
        IWriteRepository<T> GetWriteRepository<T>() where T : BaseEntity;
        
        // Tenant-Aware Repositories
        ITenantReadRepository<T> GetTenantReadRepository<T>() where T : BaseEntity, ITenantEntity;
        ITenantWriteRepository<T> GetTenantWriteRepository<T>() where T : BaseEntity, ITenantEntity;
        
        // Identity Repositories
        IApplicationUserRepository Users { get; }
        IApplicationRoleRepository Roles { get; }
        IApplicationUserRoleRepository UserRoles { get; }
        
        // Tenant Repository
        IReadRepository<Tenant> Tenants { get; }
        
        // Transaction Management
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(Guid? userId, CancellationToken cancellationToken = default); // Audit için
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
        
        // Tenant Context
        void SetCurrentTenant(Guid tenantId);
        Guid? GetCurrentTenant();
    }
}
