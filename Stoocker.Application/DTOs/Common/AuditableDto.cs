using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.DTOs.Common
{
    public abstract class AuditableDto
    {
        public DateTime CreatedAt { get; init; }
        public string? CreatedBy { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public string? UpdatedBy { get; init; }
    }
}
