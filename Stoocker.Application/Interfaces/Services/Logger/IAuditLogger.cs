using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.Interfaces.Services.Logger
{
    public interface IAuditLogger
    {
        Task LogAuditAsync(string action, string entityName, object entityId, object? oldValues = null, object? newValues = null);
    }
}
