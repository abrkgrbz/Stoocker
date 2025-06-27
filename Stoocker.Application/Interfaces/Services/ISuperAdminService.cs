using Stoocker.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Application.DTOs.Auth.Response;

namespace Stoocker.Application.Interfaces.Services
{
    public interface ISuperAdminService
    {
        Task<Result<AdminLoginResponse>> LoginAsync(string email, string password, string? ipAddress = null, string? userAgent = null);
        Task<Result> CreateSuperAdminAsync(string email, string password, string firstName, string lastName);
        Task<Result> ChangePasswordAsync(Guid adminId, string currentPassword, string newPassword);
        Task<bool> ValidateTokenAsync(string token);
    }
}
