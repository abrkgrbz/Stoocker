﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Application.Interfaces.Repositories.Entities.Customer;
using Stoocker.Persistence.Contexts;

namespace Stoocker.Persistence.Repositories.Entities.Customer
{
    public class CustomerWriteRepository:WriteRepository<Domain.Entities.Customer>,ICustomerWriteRepository
    {
        public CustomerWriteRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
