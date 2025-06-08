using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.DTOs.User.Request
{
    public class CreateUserRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(256)]
        public string Email { get; init; } = string.Empty;

        [Required(ErrorMessage = "First name is required")]
        [MaxLength(50)]
        public string FirstName { get; init; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [MaxLength(50)]
        public string LastName { get; init; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string Password { get; init; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number")]
        public string? PhoneNumber { get; init; }

        public string? TimeZone { get; init; } = "Europe/Istanbul";
        public string? Language { get; init; } = "tr-TR";

        public string DefaultRole { get; init; } = "User";
        public List<Guid>? RoleIds { get; init; }
    }
}
