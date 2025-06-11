using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Application.Interfaces.Repositories.Entities.Brand;
using Stoocker.Persistence.Contexts;

namespace Stoocker.Persistence.Repositories.Entities.Brand
{
    public class BrandWriteRepository:WriteRepository<Domain.Entities.Brand>,IBrandWriteRepository
    {
        public BrandWriteRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
