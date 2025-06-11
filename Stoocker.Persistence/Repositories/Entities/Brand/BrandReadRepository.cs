using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Stoocker.Application.Interfaces.Repositories;
using Stoocker.Application.Interfaces.Repositories.Entities.Brand;
using Stoocker.Persistence.Contexts;

namespace Stoocker.Persistence.Repositories.Entities.Brand
{
    public class BrandReadRepository : ReadRepository<Domain.Entities.Brand>, IBrandReadRepository
    {
        public BrandReadRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
