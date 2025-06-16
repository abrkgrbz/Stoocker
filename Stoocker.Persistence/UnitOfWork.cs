using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Stoocker.Application.Interfaces.Repositories;
using Stoocker.Application.Interfaces.Repositories.Entities.ApplicationRole;
using Stoocker.Application.Interfaces.Repositories.Entities.ApplicationUser;
using Stoocker.Application.Interfaces.Repositories.Entities.ApplicationUserRole;
using Stoocker.Application.Interfaces.Repositories.Entities.Tenant;
using Stoocker.Domain.Common;
using Stoocker.Domain.Entities;
using Stoocker.Persistence.Contexts;
using Stoocker.Persistence.Repositories;
using Stoocker.Persistence.Repositories.Entities.Tenant;
using Stoocker.Persistence.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Persistence.Repositories.Entities;
using Stoocker.Persistence.Repositories.Entities.Tenant.Specifications;

namespace Stoocker.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly Dictionary<string, object> _repositories;
        private IDbContextTransaction? _currentTransaction;
        private Guid? _currentTenantId;

        // Lazy-loaded repositories
        private IApplicationUserRepository? _userRepository;
        private ITenantSpecReadRepository? _tenantSpecReadRepository;
        private IApplicationRoleRepository? _roleRepository;
        private IApplicationUserRoleRepository? _userRoleRepository;
        private IReadRepository<Tenant>? _tenantRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            _repositories = new Dictionary<string, object>();
        }

        public IApplicationUserRepository Users =>
            _userRepository ??= new ApplicationUserRepository(_context);

        public IApplicationRoleRepository Roles =>
            _roleRepository ??= new ApplicationRoleRepository(_context);

        public IApplicationUserRoleRepository UserRoles =>
            _userRoleRepository ??= new ApplicationUserRoleRepository(_context);

        public IReadRepository<Tenant> Tenants =>
            _tenantRepository ??= new ReadRepository<Tenant>(_context);

        public ITenantSpecReadRepository TenantSpecs =>
            _tenantSpecReadRepository ??= new TenantSpecReadRepository(_context);

        public IReadRepository<T> GetReadRepository<T>() where T : BaseEntity
        {
            var type = typeof(T);
            var key = $"{type.Name}_Read";
            if (!_repositories.ContainsKey(key))
            {
                _repositories[key] = new ReadRepository<T>(_context);
            }
            return (IReadRepository<T>)_repositories[key];
        }

        public IWriteRepository<T> GetWriteRepository<T>() where T : BaseEntity
        {
            var type = typeof(T);
            var key = $"{type.Name}_Write";
            if (!_repositories.ContainsKey(key))
            {
                _repositories[key] = new WriteRepository<T>(_context);
            }
            return (IWriteRepository<T>)_repositories[key];
        }

        public ITenantReadRepository<T> GetTenantReadRepository<T>() where T : BaseEntity, ITenantEntity
        {
            var type = typeof(T);
            var key = $"{type.Name}_TenantRead";
            if (!_repositories.ContainsKey(key))
            {
                _repositories[key] = new TenantReadRepository<T>(_context);
            }
            return (ITenantReadRepository<T>)_repositories[key];
        }

        public ITenantWriteRepository<T> GetTenantWriteRepository<T>() where T : BaseEntity, ITenantEntity
        {
            var type = typeof(T);
            var key = $"{type.Name}_TenantWrite";
            if (!_repositories.ContainsKey(key))
            {
                _repositories[key] = new TenantWriteRepository<T>(_context);
            }
            return (ITenantWriteRepository<T>)_repositories[key];
        }

        public void SetCurrentTenant(Guid tenantId)
        {
            _currentTenantId = tenantId;
        }

        public Guid? GetCurrentTenant()
        {
            return _currentTenantId;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> SaveChangesAsync(Guid? userId, CancellationToken cancellationToken = default)
        {
            // Audit log için userId kullanılabilir
            // ChangeTracker üzerinden değişiklikleri yakala
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await SaveChangesAsync(cancellationToken);
                await _currentTransaction?.CommitAsync(cancellationToken)!;
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                _currentTransaction?.Dispose();
                _currentTransaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _currentTransaction?.RollbackAsync(cancellationToken)!;
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }

        public void Dispose()
        {
            _currentTransaction?.Dispose();
            _context.Dispose();
        }
    }
}
