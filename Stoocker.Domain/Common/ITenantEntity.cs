using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Domain.Common
{
    public interface ITenantEntity
    {
        Guid TenantId { get; set; }
    }
}
