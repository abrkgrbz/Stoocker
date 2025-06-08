using Microsoft.EntityFrameworkCore;
using Stoocker.Application.Interfaces.Repositories.Entities.ApplicationUserRole;
using Stoocker.Domain.Entities;
using Stoocker.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Persistence.Repositories.Entities
{
    public class ApplicationUserRoleRepository: IApplicationUserRoleRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<ApplicationUserRole> _userRoles;
        private readonly DbSet<ApplicationRole> _roles;

        public ApplicationUserRoleRepository(ApplicationDbContext context)
        {
            _context = context;
            _userRoles = context.Set<ApplicationUserRole>();
            _roles = context.Set<ApplicationRole>();
        }

        public async Task<ApplicationUserRole?> GetAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
        {
            return await _userRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
        }

        public async Task<IEnumerable<ApplicationUserRole>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _userRoles
                .Where(ur => ur.UserId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ApplicationUserRole>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            return await _userRoles
                .Where(ur => ur.RoleId == roleId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ApplicationUserRole>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _userRoles
                .Where(ur => ur.UserId == userId && ur.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ApplicationUserRole>> GetByUserIdWithRoleAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _userRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ApplicationUserRole>> GetByRoleIdWithUserAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            return await _userRoles
                .Include(ur => ur.User)
                .Where(ur => ur.RoleId == roleId)
                .ToListAsync(cancellationToken);
        }

        public async Task<ApplicationUserRole> AddAsync(ApplicationUserRole userRole, CancellationToken cancellationToken = default)
        {
            await _userRoles.AddAsync(userRole, cancellationToken);
            return userRole;
        }

        public async Task AddRangeAsync(IEnumerable<ApplicationUserRole> userRoles, CancellationToken cancellationToken = default)
        {
            await _userRoles.AddRangeAsync(userRoles, cancellationToken);
        }

        public void Update(ApplicationUserRole userRole)
        {
            _userRoles.Update(userRole);
        }

        public async Task<bool> RemoveAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
        {
            var userRole = await GetAsync(userId, roleId, cancellationToken);
            if (userRole == null) return false;

            _userRoles.Remove(userRole);
            return true;
        }

        public async Task RemoveAllUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var userRoles = await _userRoles
                .Where(ur => ur.UserId == userId)
                .ToListAsync(cancellationToken);

            _userRoles.RemoveRange(userRoles);
        }

        public async Task RemoveAllRoleUsersAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            var userRoles = await _userRoles
                .Where(ur => ur.RoleId == roleId)
                .ToListAsync(cancellationToken);

            _userRoles.RemoveRange(userRoles);
        }

        public async Task<bool> UserHasRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
        {
            return await _userRoles
                .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId && ur.IsActive, cancellationToken);
        }

        public async Task<bool> UserHasRoleAsync(Guid userId, string roleName, Guid tenantId, CancellationToken cancellationToken = default)
        {
            return await _userRoles
                .Include(ur => ur.Role)
                .AnyAsync(ur =>
                    ur.UserId == userId &&
                    ur.Role.Name == roleName &&
                    ur.Role.TenantId == tenantId &&
                    ur.IsActive &&
                    ur.Role.IsActive,
                    cancellationToken);
        }

        public async Task<int> GetUserCountInRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            return await _userRoles
                .CountAsync(ur => ur.RoleId == roleId && ur.IsActive, cancellationToken);
        }

        public IQueryable<ApplicationUserRole> Query()
        {
            return _userRoles;
        }
    }
}
