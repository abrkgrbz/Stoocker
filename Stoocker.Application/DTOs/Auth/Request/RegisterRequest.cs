using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.DTOs.Auth.Request
{
    public sealed record RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; init; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Username { get; init; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; init; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; init; }= string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FirstName { get; init; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Lastname { get; init; }

        [Required]
        [Phone]
        public string PhoneNumber { get; init; }

        public Guid? TenantId { get; init; } = null;
    }
}
