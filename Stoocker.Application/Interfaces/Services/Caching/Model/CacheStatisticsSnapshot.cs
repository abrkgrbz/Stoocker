using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.Interfaces.Services.Caching.Model
{
    public class CacheStatisticsSnapshot
    {
        public long TotalHits { get; set; }
        public long TotalMisses { get; set; }
        public long TotalSets { get; set; }
        public long TotalEvictions { get; set; }
        public double HitRate { get; set; }
        public List<KeyValuePair<string, long>> TopHitKeys { get; set; } = new();
        public List<KeyValuePair<string, long>> TopMissKeys { get; set; } = new();
    }
}
