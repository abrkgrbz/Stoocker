using Stoocker.Application.DTOs.Auth.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Application.DTOs.Auth.Response;
using Stoocker.Application.DTOs.Common;
using Stoocker.Domain.Entities;

namespace Stoocker.Application.Interfaces.Services
{
    public interface IAuthService
    {
        // Kullanıcı girişi
        Task<Result<LoginResponse>> LoginAsync(string email, string password);
        Task<Result<LoginResponse>> LoginAsync(LoginRequest request);

        // Kullanıcı çıkışı
        Task LogoutAsync();
        Task LogoutAsync(Guid userId);

        // Kullanıcı kaydı
        Task<Result<LoginResponse>> RegisterAsync(string email, string password, string userName, string firstName, string lastName, string phoneNumber);
        Task<Result<LoginResponse>> RegisterAsync(RegisterRequest request);

        // Token işlemleri
        Task<string> GenerateTokenAsync(ApplicationUser user);
        Task<string> RefreshTokenAsync(string refreshToken);
        Task<bool> ValidateTokenAsync(string token);
        Task RevokeTokenAsync(string token);

        // Kullanıcı doğrulama
        Task<bool> ValidateUserAsync(string email, string password); 

        // Şifre işlemleri
        Task<bool> ChangePasswordAsync(ChangePasswordRequest request);
        Task<bool> ResetPasswordAsync(string email);
        Task<bool> SetPasswordAsync(Guid userId, string newPassword);

        // Email doğrulama
        Task<bool> SendEmailVerificationAsync(Guid userId);
        Task<bool> VerifyEmailAsync(Guid userId, string verificationCode);
 
    }
}
