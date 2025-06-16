using Microsoft.EntityFrameworkCore;
using Stoocker.Application.Interfaces.Repositories.Entities.ApplicationUser;
using Stoocker.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Domain.Entities;

namespace Stoocker.Persistence.Repositories.Entities
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<ApplicationUser> _users;

        public ApplicationUserRepository(ApplicationDbContext context)
        {
            _context = context;
            _users = context.Set<ApplicationUser>();
        }

        public async Task<ApplicationUser?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _users.FindAsync(new object[] { userId }, cancellationToken);
        }

        public async Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _users
                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted, cancellationToken);
        }

        public Task<ApplicationUser?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<ApplicationUser?> GetByIdAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default)
        {
            return await _users
                .FirstOrDefaultAsync(u => u.Id == userId && u.TenantId == tenantId && !u.IsDeleted, cancellationToken);
        }

        public async Task<ApplicationUser?> GetByEmailAsync(string email, Guid tenantId, CancellationToken cancellationToken = default)
        {
            return await _users
                .FirstOrDefaultAsync(u => u.Email == email && u.TenantId == tenantId && !u.IsDeleted, cancellationToken);
        }

        public async Task<ApplicationUser?> GetWithRolesAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllByTenantAsync(Guid tenantId, bool includeInactive = false, CancellationToken cancellationToken = default)
        {
            var query = _users.Where(u => u.TenantId == tenantId && !u.IsDeleted);

            if (!includeInactive)
                query = query.Where(u => u.IsActive);

            return await query.ToListAsync(cancellationToken);
        }

        public IQueryable<ApplicationUser> Query()
        {
            return _users.Where(u => !u.IsDeleted);
        }

        public IQueryable<ApplicationUser>  (Guid tenantId)
        {
            return _users.Where(u => u.TenantId == tenantId && !u.IsDeleted);
        }

        public async Task<ApplicationUser> AddAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            await _users.AddAsync(user, cancellationToken);
            return user;
        }

        public void Update(Domain.Entities.ApplicationUser user)
        {
            _users.Update(user);
        }

        public async Task<bool> SoftDeleteAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default)
        {
            var user = await GetByIdAsync(userId, tenantId, cancellationToken);
            if (user == null) return false;

            user.IsDeleted = true;
            user.UpdatedAt = DateTime.UtcNow;
            Update(user);

            return true;
        }

        public async Task<bool> IsEmailUniqueAsync(string email, Guid tenantId, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
        {
            var query = _users.Where(u => u.Email == email && u.TenantId == tenantId && !u.IsDeleted);

            if (excludeUserId.HasValue)
                query = query.Where(u => u.Id != excludeUserId.Value);

            return !await query.AnyAsync(cancellationToken);
        }

        public async Task<bool> IsUserNameUniqueAsync(string userName, Guid tenantId, Guid? excludeUserId = null,
            CancellationToken cancellationToken = default)
        {
            return await _users.Where(u => u.UserName == userName && u.TenantId == tenantId && !u.IsDeleted)
                .Where(u => !excludeUserId.HasValue || u.Id != excludeUserId.Value)
                .AnyAsync(cancellationToken);
        }

        public async Task<int> GetActiveUserCountAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            return await _users.CountAsync(u => u.TenantId == tenantId && u.IsActive && !u.IsDeleted, cancellationToken);
        }

        public async Task<int> CountAsync(Expression<Func<ApplicationUser, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            return predicate == null
                ? await Query().CountAsync(cancellationToken)
                : await Query().CountAsync(predicate, cancellationToken);
        }
    }
}
