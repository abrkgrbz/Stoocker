using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Stoocker.Application.DTOs.Auth.Request;
using Stoocker.Application.DTOs.Auth.Response;
using Stoocker.Application.DTOs.Common;
using Stoocker.Application.Interfaces.Repositories;
using Stoocker.Application.Interfaces.Services;
using Stoocker.Domain.Entities;

namespace Stoocker.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserService _userService;
        private readonly ILogger<AuthService> _logger;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;


        private readonly Dictionary<string, string> _verificationCodes = new();

        public AuthService(ICurrentUserService currentUserService, IUserService userService,
            ILogger<AuthService> logger, IEmailService emailService, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _currentUserService = currentUserService;
            _userService = userService;
            _logger = logger;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<Result<LoginResponse>> LoginAsync(string email, string password)
        {
            return await LoginAsync(new LoginRequest { Email = email, Password = password });
        }

        public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _userService.GetByEmailAsync(request.Email);
                if (!user.IsSuccess)
                {
                    return Result<LoginResponse>.Failure("Invalid email or password");
                }

                if (!VerifyPassword(request.Password, user.Data.PasswordHash))
                {
                    return Result<LoginResponse>.Failure("Invalid email or password");
                }

                if (!user.Data.EmailConfirmed)
                {
                    return Result<LoginResponse>.Failure("Please verify your email first");
                }

                var token = await GenerateTokenAsync(user.Data);
                var refreshToken = GenerateRefreshToken();

                // Update refresh token
                user.Data.RefreshToken = refreshToken;
                user.Data.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7); 
                _unitOfWork.Users.Update(user.Data);
                await _unitOfWork.SaveChangesAsync();

                var expiresAt = DateTime.UtcNow.AddHours(1); // Token expires in 1 hour
                _unitOfWork.SetCurrentTenant(user.Data.TenantId);
                var result = new LoginResponse()
                {
                    AccessToken = token,
                    RefreshToken = refreshToken,
                    User = new LoginResponse.UserInfo()
                    {
                        Id = user.Data.Id,
                        Email = user.Data.Email,
                        FullName = user.Data.FullName,
                        TenantId = user.Data.TenantId,
                        TenantName = user.Data.Tenant?.Name ?? "Default Tenant",
                        Roles = user.Data.UserRoles.Select(ur => ur.Role.Name).ToList()
                    },
                    ExpiresAt = expiresAt
                };

                return Result<LoginResponse>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
                return Result<LoginResponse>.Failure("An error occurred during login");
            }
        }

        public async Task LogoutAsync()
        {
            var currentUser = _currentUserService.UserId;
            if (currentUser != null)
            {
                await LogoutAsync(currentUser.Value);
            }
        }

        public async Task LogoutAsync(Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user != null)
                {
                    user.RefreshToken = null;
                    user.RefreshTokenExpiry = null;
                    _unitOfWork.Users.Update(user);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout for user: {UserId}", userId);
            }
        }

        public async Task<Result<LoginResponse>> RegisterAsync(string email, string password,string userName,string firstName,string lastName,string phoneNumber)
        {
            return await RegisterAsync(new RegisterRequest { Email = email, Password = password, ConfirmPassword = password,FirstName = firstName,Lastname = lastName,PhoneNumber = phoneNumber,Username = userName});
        }

        public async Task<Result<LoginResponse>> RegisterAsync(RegisterRequest request)
        {
            try
            {
                if (request.Password != request.ConfirmPassword)
                {
                    return Result<LoginResponse>.Failure("Passwords do not match");
                }

                // Tenant kontrolü
                var currentTenant = await _unitOfWork.Tenants.FirstOrDefaultAsync(x=>x.Id== request.TenantId);
                if (currentTenant == null || !currentTenant.IsActive)
                {
                    return  Result<LoginResponse>.Failure("Invalid tenant");
                }
                 
                var existingUser = await _unitOfWork.Users.GetByEmailAsync(request.Email, currentTenant.Id);
                if (existingUser != null)
                {
                    return Result<LoginResponse>.Failure("User already exists in this organization"); 
                }

                var user = new ApplicationUser()
                {
                    Id = Guid.NewGuid(),
                    TenantId = currentTenant.Id, // Tenant ID ekleme
                    Email = request.Email,
                    PasswordHash = HashPassword(request.Password),
                    EmailConfirmed = false,
                    TwoFactorEnabled = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Users.AddAsync(user);
                await SendEmailVerificationAsync(user.Id);
                await _unitOfWork.SaveChangesAsync();
                var loginResponse = new LoginResponse()
                {
                    User = new LoginResponse.UserInfo()
                    {
                        Email = user.Email,
                        FullName = user.FullName,
                        TenantName = user.Tenant.Name,
                        Id = user.Id
                    }
                };

                return Result<LoginResponse>.Success(loginResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email: {Email}", request.Email);
                return Result<LoginResponse>.Failure("An error occurred during registration");
            }
        }

        public async Task<string> GenerateTokenAsync(ApplicationUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("TenantId",user.TenantId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var roles = await _unitOfWork.UserRoles.GetByUserIdWithRoleAsync(user.Id);
             foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Role.Name));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var user = await _userService.GetByRefreshTokenAsync(refreshToken);
                if (user == null || user.Data.RefreshTokenExpiry < DateTime.UtcNow)
                {
                    throw new SecurityTokenException("Invalid refresh token");
                }

                return await GenerateTokenAsync(user.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                throw;
            }
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task RevokeTokenAsync(string token)
        {
            // In production, you might want to maintain a blacklist of revoked tokens
            // For now, we'll just log the action
            _logger.LogInformation("Token revoked: {Token}", token[..10] + "...");
            await Task.CompletedTask;
        }

        public async Task<bool> ValidateUserAsync(string email, string password)
        {
            var user = await _userService.GetByEmailAsync(email);
            return user.Data != null && VerifyPassword(password, user.Data.PasswordHash);
        }


        public async Task<bool> ChangePasswordAsync(ChangePasswordRequest request)
        {
            if (!_currentUserService.UserId.HasValue)
            {
                _logger.LogWarning("Attempted to change password without authenticated user");
                return false;
            }

            var currentUserId = _currentUserService.UserId.Value;

            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(currentUserId);
                if (user == null || !VerifyPassword(request.oldPassword, user.PasswordHash))
                {
                    return false;
                }

                user.PasswordHash = HashPassword(request.newPassword);
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user: {UserId}", currentUserId);
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(string email)
        {
            try
            {
                var user = await _userService.GetByEmailAsync(email);
                if (user.Data == null) return false;

                var resetCode = GenerateVerificationCode();
                _verificationCodes[user.Data.Id.ToString()] = resetCode;

                await _emailService.SendPasswordResetEmailAsync(email, resetCode);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending password reset for email: {Email}", email);
                return false;
            }
        }

        public async Task<bool> SetPasswordAsync(Guid userId, string newPassword)
        {
            
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null) return false;

                user.PasswordHash = HashPassword(newPassword);
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting password for user: {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> SendEmailVerificationAsync(Guid userId)
        { 
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null) return false;

                var verificationCode = GenerateVerificationCode();
                _verificationCodes[userId.ToString()] = verificationCode;

                await _emailService.SendEmailVerificationAsync(user.Email, verificationCode);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email verification for user: {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> VerifyEmailAsync(Guid userId, string verificationCode)
        {
            try
            {
                if (!_verificationCodes.TryGetValue(userId.ToString(), out var storedCode) || storedCode != verificationCode)
                {
                    return false;
                }

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null) return false;

                user.EmailConfirmed = true; 
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();
                _verificationCodes.Remove(userId.ToString());
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying email for user: {UserId}", userId);
                return false;
            }
        }

       

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        private string GenerateVerificationCode()
        {
            return new Random().Next(100000, 999999).ToString();
        }
    }
}
