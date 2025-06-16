using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Application.Interfaces.Repositories.Entities.Category;
using Stoocker.Persistence.Contexts;
using Stoocker.Persistence.Repositories.Specification;

namespace Stoocker.Persistence.Repositories.Entities.Category
{
    public class CategoryReadRepository: SpecificationRepository<Domain.Entities.Category>,ICategoryReadRepository
    {
        public CategoryReadRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
