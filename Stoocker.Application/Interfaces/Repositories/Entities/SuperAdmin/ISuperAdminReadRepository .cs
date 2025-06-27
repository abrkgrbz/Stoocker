using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.Interfaces.Repositories.Entities.SuperAdmin
{
    public interface ISuperAdminReadRepository : IReadRepository<Domain.Entities.SuperAdmin>
    {
        Task<Domain.Entities.SuperAdmin?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> CheckPasswordAsync(Domain.Entities.SuperAdmin admin, string password);
    }
}
