using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Stoocker.Application.Interfaces.Repositories.Entities.ApplicationRole;
using Stoocker.Domain.Entities;
using Stoocker.Persistence.Contexts;

namespace Stoocker.Persistence.Repositories.Entities
{
    public class ApplicationRoleRepository : IApplicationRoleRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<ApplicationRole> _roles;

        public ApplicationRoleRepository(ApplicationDbContext context)
        {
            _context = context;
            _roles = context.Set<ApplicationRole>();
        }

        // Read Operations
        public async Task<ApplicationRole?> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            return await _roles
                .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);
        }

        public async Task<ApplicationRole?> GetByNameAsync(string roleName, Guid tenantId, CancellationToken cancellationToken = default)
        {
            return await _roles
                .FirstOrDefaultAsync(r =>
                    r.Name == roleName &&
                    r.TenantId == tenantId &&
                    r.IsActive,
                    cancellationToken);
        }

        public async Task<ApplicationRole?> GetWithUsersAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            return await _roles
                .Include(r => r.UserRoles)
                    .ThenInclude(ur => ur.User)
                .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);
        }

        // Tenant-specific Operations
        public async Task<IEnumerable<ApplicationRole>> GetAllByTenantAsync(Guid tenantId, bool includeSystemRoles = false, CancellationToken cancellationToken = default)
        {
            var query = _roles.Where(r => r.TenantId == tenantId);

            if (!includeSystemRoles)
            {
                query = query.Where(r => !r.IsSystemRole);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ApplicationRole>> GetActiveRolesByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            return await _roles
                .Where(r => r.TenantId == tenantId && r.IsActive)
                .OrderBy(r => r.Name)
                .ToListAsync(cancellationToken);
        }

        // Query Operations
        public IQueryable<ApplicationRole> Query()
        {
            return _roles;
        }

        public IQueryable<ApplicationRole> QueryByTenant(Guid tenantId)
        {
            return _roles.Where(r => r.TenantId == tenantId);
        }

        // Write Operations
        public async Task<ApplicationRole> AddAsync(ApplicationRole role, CancellationToken cancellationToken = default)
        {
            await _roles.AddAsync(role, cancellationToken);
            return role;
        }

        public void Update(ApplicationRole role)
        {
            _roles.Update(role);
        }

        public async Task<bool> DeleteAsync(Guid roleId, Guid tenantId, CancellationToken cancellationToken = default)
        {
            var role = await _roles
                .FirstOrDefaultAsync(r => r.Id == roleId && r.TenantId == tenantId, cancellationToken);

            if (role == null)
                return false;

            // System rolleri silinemez
            if (role.IsSystemRole)
                throw new InvalidOperationException("System roles cannot be deleted");

            // Soft delete için IsActive = false yapabiliriz
            // veya hard delete:
            _roles.Remove(role);

            return true;
        }

        // Validation Operations
        public async Task<bool> IsRoleNameUniqueAsync(string roleName, Guid tenantId, Guid? excludeRoleId = null, CancellationToken cancellationToken = default)
        {
            var query = _roles.Where(r =>
                r.Name == roleName &&
                r.TenantId == tenantId);

            if (excludeRoleId.HasValue)
            {
                query = query.Where(r => r.Id != excludeRoleId.Value);
            }

            return !await query.AnyAsync(cancellationToken);
        }

        public async Task<bool> CanDeleteRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            var role = await GetWithUsersAsync(roleId, cancellationToken);

            if (role == null)
                return false;

            // System rolleri silinemez
            if (role.IsSystemRole)
                return false;

            // Aktif kullanıcısı olan roller silinemez
            var hasActiveUsers = role.UserRoles.Any(ur => ur.IsActive);

            return !hasActiveUsers;
        }
    }
}
