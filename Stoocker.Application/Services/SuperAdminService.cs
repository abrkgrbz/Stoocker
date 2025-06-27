using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Stoocker.Application.DTOs.Common;
using Stoocker.Application.Interfaces.Repositories.Entities.SuperAdmin;
using Stoocker.Application.Interfaces.Repositories;
using Stoocker.Application.Interfaces.Services;
using Stoocker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Application.DTOs.Auth.Response;

namespace Stoocker.Application.Services
{
    public class SuperAdminService : ISuperAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISuperAdminReadRepository _readRepository;
        private readonly ISuperAdminWriteRepository _writeRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SuperAdminService> _logger;

        public SuperAdminService(
            IUnitOfWork unitOfWork,
            ISuperAdminReadRepository readRepository,
            ISuperAdminWriteRepository writeRepository,
            IConfiguration configuration,
            ILogger<SuperAdminService> logger)
        {
            _unitOfWork = unitOfWork;
            _readRepository = readRepository;
            _writeRepository = writeRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<Result<AdminLoginResponse>> LoginAsync(string email, string password, string? ipAddress = null, string? userAgent = null)
        {
            try
            {
                var admin = await _readRepository.GetByEmailAsync(email);
                if (admin == null)
                {
                    _logger.LogWarning("SuperAdmin login failed - user not found: {Email}", email);
                    return Result<AdminLoginResponse>.Failure("Geçersiz kullanıcı adı veya şifre");
                }

                // Lockout kontrolü
                if (admin.LockoutEndDate.HasValue && admin.LockoutEndDate > DateTime.UtcNow)
                {
                    var remainingTime = (admin.LockoutEndDate.Value - DateTime.UtcNow).TotalMinutes;
                    return Result<AdminLoginResponse>.Failure($"Hesabınız kilitli. {Math.Ceiling(remainingTime)} dakika sonra tekrar deneyin.");
                }

                // Şifre kontrolü
                if (!VerifyPassword(password, admin.PasswordHash))
                {
                    await _writeRepository.IncrementFailedLoginAsync(admin.Id);

                    if (admin.FailedLoginCount >= 4) // 5. başarısız deneme
                    {
                        admin.LockoutEndDate = DateTime.UtcNow.AddMinutes(30);
                        await _unitOfWork.SaveChangesAsync();
                    }

                    return Result<AdminLoginResponse>.Failure("Geçersiz kullanıcı adı veya şifre");
                }

                // Aktif kontrolü
                if (!admin.IsActive)
                {
                    return Result<AdminLoginResponse>.Failure("Hesabınız aktif değil");
                }

                // Başarılı giriş
                await _writeRepository.ResetFailedLoginAsync(admin.Id);
                await _writeRepository.UpdateLastLoginAsync(admin.Id, ipAddress, userAgent);

                // Token oluştur
                var token = GenerateSuperAdminToken(admin);
                var refreshToken = GenerateRefreshToken();

                admin.RefreshToken = refreshToken;
                admin.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _unitOfWork.SaveChangesAsync();

                var response = new AdminLoginResponse()
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    AdminId = admin.Id,
                    Email = admin.Email,
                    FullName = $"{admin.FirstName} {admin.LastName}" 
                };

                _logger.LogInformation("SuperAdmin login successful: {Email} from {IP}", email, ipAddress);
                return Result<AdminLoginResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SuperAdmin login error");
                return Result<AdminLoginResponse>.Failure("Giriş sırasında bir hata oluştu");
            }
        }

        private string GenerateSuperAdminToken(SuperAdmin admin)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                new Claim(ClaimTypes.Email, admin.Email),
                new Claim(ClaimTypes.Name, $"{admin.FirstName} {admin.LastName}"),
                new Claim(ClaimTypes.Role, "SuperAdmin"),
                new Claim("IsSuperAdmin", "true"),
                new Claim("AdminType", "System"),
                new Claim("SecurityStamp", admin.SecurityStamp)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: "SuperAdminPanel",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(4), // Daha kısa süre
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, 12);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<Result> CreateSuperAdminAsync(string email, string password, string firstName, string lastName)
        { 
            var existingAdmin = await _readRepository.GetByEmailAsync(email);
            if (existingAdmin != null)
            {
                return Result.Failure("Bu email adresi zaten kullanımda");
            }

            var admin = new SuperAdmin
            {
                Id = Guid.NewGuid(),
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                PasswordHash = HashPassword(password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _writeRepository.AddAsync(admin);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("New SuperAdmin created: {Email}", email);
            return Result.Success();
        }

        public Task<bool> ValidateTokenAsync(string token)
        { 
            throw new NotImplementedException();
        }

        public Task<Result> ChangePasswordAsync(Guid adminId, string currentPassword, string newPassword)
        { 
            throw new NotImplementedException();
        }
    }

    public class SuperAdminLoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public Guid AdminId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool TwoFactorRequired { get; set; }
    }

}
