using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Application.Interfaces.Repositories.Entities.RolePermission;
using Stoocker.Persistence.Contexts;

namespace Stoocker.Persistence.Repositories.Entities.RolePermission
{
    public class RolePermissionReadRepository:ReadRepository<Domain.Entities.RolePermission>,IRolePermissionReadRepository
    {
        public RolePermissionReadRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
