using Stoocker.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.DTOs.User.Request
{
    public sealed class UserFilterRequest : PagedRequest
    {
        public string? Name { get; init; }
        public string? Email { get; init; }
        public bool? IsActive { get; init; }
        public List<Guid>? RoleIds { get; init; }
        public DateTime? CreatedFrom { get; init; }
        public DateTime? CreatedTo { get; init; }
    }
}
