using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.Interfaces.Repositories.Entities.SuperAdmin
{
    public interface ISuperAdminWriteRepository : IWriteRepository<Domain.Entities.SuperAdmin>
    {
        Task<bool> UpdatePasswordAsync(Guid adminId, string newPasswordHash);
        Task<bool> UpdateLastLoginAsync(Guid adminId, string? ipAddress = null, string? userAgent = null);
        Task<bool> IncrementFailedLoginAsync(Guid adminId);
        Task<bool> ResetFailedLoginAsync(Guid adminId);
    }
}
