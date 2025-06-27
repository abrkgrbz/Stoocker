using Stoocker.Application.Interfaces.Repositories.Entities.SuperAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Stoocker.Persistence.Repositories.Entities.SuperAdmin
{
    public class SuperAdminWriteRepository:WriteRepository<Domain.Entities.SuperAdmin>,ISuperAdminWriteRepository
    {
        private readonly DbSet<Domain.Entities.SuperAdmin> _superAdmins;

        public SuperAdminWriteRepository(ApplicationDbContext context) : base(context)
        {
            _superAdmins = context.Set<Domain.Entities.SuperAdmin>();
        }

        public async Task<bool> UpdatePasswordAsync(Guid adminId, string newPasswordHash)
        {
            var result = await _superAdmins
                .Where(x => x.Id == adminId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.PasswordHash, newPasswordHash)
                    .SetProperty(x => x.UpdatedAt, DateTime.UtcNow)
                    .SetProperty(x => x.SecurityStamp, Guid.NewGuid().ToString())
                );

            return result > 0;
        }

        public async Task<bool> UpdateLastLoginAsync(Guid adminId, string? ipAddress = null, string? userAgent = null)
        {
            var query = _superAdmins.Where(x => x.Id == adminId);

            var result = await query.ExecuteUpdateAsync(s => s
                .SetProperty(x => x.LastLoginDate, DateTime.UtcNow)
                .SetProperty(x => x.LastLoginIp, v => ipAddress ?? v.LastLoginIp)
                .SetProperty(x => x.LastLoginUserAgent, v => userAgent ?? v.LastLoginUserAgent)
            );

            return result > 0;
        }

        public async Task<bool> IncrementFailedLoginAsync(Guid adminId)
        {
            // Önce mevcut değeri oku
            var admin = await _superAdmins
                .Where(x => x.Id == adminId)
                .Select(x => new { x.Id, x.FailedLoginCount })
                .FirstOrDefaultAsync();

            if (admin == null)
                return false;

            var newFailedCount = admin.FailedLoginCount + 1;
            var lockoutEnd = newFailedCount >= 5 ? DateTime.UtcNow.AddMinutes(30) : (DateTime?)null;

            var result = await _superAdmins
                .Where(x => x.Id == adminId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.FailedLoginCount, newFailedCount)
                    .SetProperty(x => x.LockoutEndDate, lockoutEnd)
                );

            return result > 0;
        }

        public async Task<bool> ResetFailedLoginAsync(Guid adminId)
        {
            var result = await _superAdmins
                .Where(x => x.Id == adminId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.FailedLoginCount, 0)
                    .SetProperty(x => x.LockoutEndDate, (DateTime?)null)
                );

            return result > 0;
        }
    }
}
