﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.Interfaces.Services.Caching
{
    public interface IMemoryCacheService : ICacheService
    {
        void Clear();
    }
}
