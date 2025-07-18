﻿using Stoocker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Application.Interfaces.Repositories.Entities.ProductStock;
using Stoocker.Persistence.Contexts;

namespace Stoocker.Persistence.Repositories.Entities.ProductStock
{
    public class ProductStockWriteRepository:WriteRepository<Domain.Entities.ProductStock>, IProductStockWriteRepository
    {
        public ProductStockWriteRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
