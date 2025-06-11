using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Application.Interfaces.Repositories.Entities.Category;
using Stoocker.Persistence.Contexts;

namespace Stoocker.Persistence.Repositories.Entities.Category
{
    public class CategoryReadRepository:ReadRepository<Domain.Entities.Category>,ICategoryReadRepository
    {
        public CategoryReadRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
