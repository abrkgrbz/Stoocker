using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Stoocker.Application.Interfaces.Repositories.Entities.SuperAdmin;
using Stoocker.Persistence.Contexts;

namespace Stoocker.Persistence.Repositories.Entities.SuperAdmin
{
    public class SuperAdminReadRepository:ReadRepository<Domain.Entities.SuperAdmin>, ISuperAdminReadRepository
    {
        private readonly DbSet<Domain.Entities.SuperAdmin> _superAdmins;
        public SuperAdminReadRepository(ApplicationDbContext context) : base(context)
        {
            _superAdmins = context.Set<Domain.Entities.SuperAdmin>();
        }

        public async Task<Domain.Entities.SuperAdmin?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _superAdmins
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower(), cancellationToken);
        }

        public async Task<bool> CheckPasswordAsync(Domain.Entities.SuperAdmin admin, string password)
        {
            try
            { 
                var isValid = BCrypt.Net.BCrypt.Verify(password, admin.PasswordHash);
                return await Task.FromResult(isValid);
            }
            catch (Exception)
            {
                // Hatalı hash formatı vs.
                return await Task.FromResult(false);
            }
        }
    }
}
