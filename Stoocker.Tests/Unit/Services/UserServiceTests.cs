using AutoMapper;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Stoocker.Application.DTOs.User.Request;
using Stoocker.Application.Interfaces.Repositories;
using Stoocker.Application.Services;
using Stoocker.Domain.Entities;
using Stoocker.Tests.Common.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Tests.Unit.Services
{
    public class UserServiceTests : IDisposable
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<UserService>> _loggerMock;
        private readonly Mock<IValidator<CreateUserRequest>> _createValidatorMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userManagerMock = GetMockUserManager();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<UserService>>();
            _createValidatorMock = new Mock<IValidator<CreateUserRequest>>();

            _userService = new UserService(
                _unitOfWorkMock.Object,
                _userManagerMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _createValidatorMock.Object
            );
        }

        [Fact]
        public async Task CreateUserAsync_WithValidData_ShouldCreateUserSuccessfully()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var tenant = TestDataBuilder.TenantBuilder.Create(t =>
            {
                t.Id = tenantId;
                t.MaxUsers = 10;
            });

            var createUserRequest = new CreateUserRequest
            {
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                Password = "Test123!",
                PhoneNumber = "+905551234567",
                TimeZone = "Europe/Istanbul",
                Language = "tr-TR",
                DefaultRole = "User"
            };

            var newUser = TestDataBuilder.UserBuilder.Create(tenantId, u =>
            {
                u.Email = createUserRequest.Email;
                u.FirstName = createUserRequest.FirstName;
                u.LastName = createUserRequest.LastName;
                u.UserName = createUserRequest.Email;
            });

            var userResponse = new Application.DTOs.User.Response.UserResponse
            {
                Id = newUser.Id,
                Email = newUser.Email,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                FullName = $"{newUser.FirstName} {newUser.LastName}",
                IsActive = true,
                Roles = new List<string> { "User" }
            };

            // Mock Setup - Validation Success
            _createValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<CreateUserRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            // Mock Setup - Tenant Exists
            _unitOfWorkMock
                .Setup(u => u.Tenants.GetByIdAsync(tenantId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(tenant);

            // Mock Setup - Current User Count (Under Limit)
            _unitOfWorkMock
                .Setup(u => u.Users.GetActiveUserCountAsync(tenantId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(5); // 10 limit altında

            // Mock Setup - Email is Unique
            _unitOfWorkMock
                .Setup(u => u.Users.IsEmailUniqueAsync(
                    createUserRequest.Email,
                    tenantId,
                    null,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Mock Setup - Mapper for Request to Entity
            _mapperMock
                .Setup(m => m.Map<ApplicationUser>(It.IsAny<CreateUserRequest>()))
                .Returns(newUser);

            // Mock Setup - Mapper for Entity to Response
            _mapperMock
                .Setup(m => m.Map<Application.DTOs.User.Response.UserResponse>(It.IsAny<ApplicationUser>()))
                .Returns(userResponse);

            // Mock Setup - UserManager CreateAsync Success
            _userManagerMock
                .Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Mock Setup - UserManager AddToRoleAsync Success
            _userManagerMock
                .Setup(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Mock Setup - SaveChangesAsync
            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _userService.CreateUserAsync(createUserRequest, tenantId);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().BeNull();
            result.Data.Should().NotBeNull();

            // Assert Response Data
            result.Data!.Id.Should().Be(userResponse.Id);
            result.Data.Email.Should().Be(createUserRequest.Email);
            result.Data.FirstName.Should().Be(createUserRequest.FirstName);
            result.Data.LastName.Should().Be(createUserRequest.LastName);
            result.Data.FullName.Should().Be($"{createUserRequest.FirstName} {createUserRequest.LastName}");
            result.Data.IsActive.Should().BeTrue();
            result.Data.Roles.Should().Contain("User");

            // Verify Method Calls
            _createValidatorMock.Verify(
                v => v.ValidateAsync(
                    It.Is<CreateUserRequest>(r => r.Email == createUserRequest.Email),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                u => u.Tenants.GetByIdAsync(tenantId, It.IsAny<CancellationToken>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                u => u.Users.GetActiveUserCountAsync(tenantId, It.IsAny<CancellationToken>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                u => u.Users.IsEmailUniqueAsync(
                    createUserRequest.Email,
                    tenantId,
                    null,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _mapperMock.Verify(
                m => m.Map<ApplicationUser>(
                    It.Is<CreateUserRequest>(r => r.Email == createUserRequest.Email)),
                Times.Once);

            _userManagerMock.Verify(
                u => u.CreateAsync(
                    It.Is<ApplicationUser>(user =>
                        user.Email == createUserRequest.Email &&
                        user.TenantId == tenantId),
                    createUserRequest.Password),
                Times.Once);

            _userManagerMock.Verify(
                u => u.AddToRoleAsync(
                    It.Is<ApplicationUser>(user => user.Email == createUserRequest.Email),
                    createUserRequest.DefaultRole),
                Times.Once);

            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);

            _mapperMock.Verify(
                m => m.Map<Application.DTOs.User.Response.UserResponse>(
                    It.Is<ApplicationUser>(user => user.Email == createUserRequest.Email)),
                Times.Once);

            // Verify Logger (if needed)
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Created user")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        private static Mock<UserManager<ApplicationUser>> GetMockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mgr = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null,
                null);
            mgr.Object.UserValidators.Add(new UserValidator<ApplicationUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<ApplicationUser>());
            return mgr;
        }

        public void Dispose()
        {
            // Cleanup if needed
        }
    }
}