using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Application.Interfaces.Repositories.Entities.Permission;
using Stoocker.Persistence.Contexts;

namespace Stoocker.Persistence.Repositories.Entities.Permission
{
    public class PermissionReadRepository:ReadRepository<Domain.Entities.Permission>,IPermissionReadRepository
    {
        public PermissionReadRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
