using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.Interfaces.Services.Caching
{
    public interface ICacheInvalidatorCommand
    {
        IEnumerable<string>? CacheKeysToInvalidate { get; }
        IEnumerable<string>? CachePatternsToInvalidate { get; }
    }
}
