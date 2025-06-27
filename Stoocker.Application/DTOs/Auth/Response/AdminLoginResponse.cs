using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Stoocker.Application.DTOs.Auth.Response.LoginResponse;

namespace Stoocker.Application.DTOs.Auth.Response
{
    public sealed class AdminLoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public Guid AdminId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; 
        public string RefreshToken { get; init; } = string.Empty;  

    }
}
