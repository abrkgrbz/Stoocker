using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.DTOs.User.Request
{
    public class UpdateUserRequest
    {
        [Required]
        public Guid Id { get; init; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; init; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; init; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; init; }

        public string? TimeZone { get; init; }
        public string? Language { get; init; }
        public bool IsActive { get; init; }
    }
}
