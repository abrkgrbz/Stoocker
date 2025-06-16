using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.DTOs.Auth.Request
{
    public sealed record ChangePasswordRequest
    {
        [Required]
        [DataType(DataType.Password)]
        public string oldPassword { get; init; }= string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string newPassword { get; init; } = string.Empty;
    }
}
