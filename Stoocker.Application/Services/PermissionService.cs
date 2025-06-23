using Microsoft.Extensions.Caching.Memory;
using Stoocker.Application.Interfaces.Repositories;
using Stoocker.Application.Interfaces.Services;
using Stoocker.Domain.Constants;
using Stoocker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Stoocker.Application.Interfaces.Services.Permission;

namespace Stoocker.Application.Services
{
    public class PermissionService:IPermissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;
        private readonly ICurrentUserService _currentUserService;
        private const string PERMISSION_CACHE_KEY = "user_permissions_{0}";
        private const int CACHE_DURATION_MINUTES = 30;

        public PermissionService(
            IUnitOfWork unitOfWork,
            IMemoryCache cache,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
            _currentUserService = currentUserService;
        }

        public async Task<bool> HasPermissionAsync(Guid userId, string permission)
        {
            var permissions = await GetUserPermissionsAsync(userId);
            return permissions.Contains(permission);
        }

        public async Task<bool> HasPermissionsAsync(Guid userId, params string[] permissions)
        {
            var userPermissions = await GetUserPermissionsAsync(userId);
            return permissions.All(p => userPermissions.Contains(p));
        }

        public async Task<bool> HasAnyPermissionAsync(Guid userId, params string[] permissions)
        {
            var userPermissions = await GetUserPermissionsAsync(userId);
            return permissions.Any(p => userPermissions.Contains(p));
        }

        public async Task<List<string>> GetUserPermissionsAsync(Guid userId)
        {
            var cacheKey = string.Format(PERMISSION_CACHE_KEY, userId);

            if (_cache.TryGetValue<List<string>>(cacheKey, out var cachedPermissions))
            {
                return cachedPermissions!;
            }

            // Get user roles
            var userRoles = await _unitOfWork.UserRoles.Query()
                .Where(ur => ur.UserId == userId && ur.IsActive)
                .Include(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .ToListAsync();

            var permissions = new List<string>();

            foreach (var userRole in userRoles)
            {
                if (userRole.Role.IsActive)
                {
                    var rolePermissions = userRole.Role.RolePermissions
                        .Where(rp => rp.IsGranted && rp.Permission.IsActive)
                        .Select(rp => rp.Permission.Name);

                    permissions.AddRange(rolePermissions);
                }
            }

            permissions = permissions.Distinct().ToList();

            _cache.Set(cacheKey, permissions, TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));

            return permissions;
        }

        public async Task<Dictionary<string, bool>> CheckPermissionsAsync(Guid userId, params string[] permissions)
        {
            var userPermissions = await GetUserPermissionsAsync(userId);
            var result = new Dictionary<string, bool>();

            foreach (var permission in permissions)
            {
                result[permission] = userPermissions.Contains(permission);
            }

            return result;
        }

        public async Task GrantPermissionToRoleAsync(Guid roleId, string permissionName)
        {
            var permission = await _unitOfWork.GetReadRepository<Permission>()
                .FirstOrDefaultAsync(p => p.Name == permissionName);

            if (permission == null)
                throw new Exception($"Permission '{permissionName}' not found");

            var existingRolePermission = await _unitOfWork.GetReadRepository<RolePermission>()
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permission.Id);

            if (existingRolePermission != null)
            {
                existingRolePermission.IsGranted = true;
                existingRolePermission.UpdatedAt = DateTime.UtcNow;
                existingRolePermission.UpdatedBy = _currentUserService.UserId;

                _unitOfWork.GetWriteRepository<RolePermission>().Update(existingRolePermission);
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                var rolePermission = new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permission.Id,
                    IsGranted = true,
                    GrantedBy = _currentUserService.UserId
                };

                await _unitOfWork.GetWriteRepository<RolePermission>().AddAsync(rolePermission);
                await _unitOfWork.SaveChangesAsync();
            }

            await _unitOfWork.SaveChangesAsync();
            ClearUserPermissionCache(roleId);
        }

        public async Task RevokePermissionFromRoleAsync(Guid roleId, string permissionName)
        {
            var permission = await _unitOfWork.GetReadRepository <Permission>()
                .FirstOrDefaultAsync(p => p.Name == permissionName);

            if (permission == null)
                throw new Exception($"Permission '{permissionName}' not found");

            var rolePermission = await _unitOfWork.GetReadRepository <RolePermission>()
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permission.Id);

            if (rolePermission != null)
            {
                rolePermission.IsGranted = false;
                rolePermission.UpdatedAt = DateTime.UtcNow;
                rolePermission.UpdatedBy = _currentUserService.UserId;

                 _unitOfWork.GetWriteRepository<RolePermission>().Update(rolePermission);
                await _unitOfWork.SaveChangesAsync();

                ClearUserPermissionCache(roleId);
            }
        }

        public async Task<List<Permission>> GetRolePermissionsAsync(Guid roleId)
        {
            var rolePermissions = await _unitOfWork.GetReadRepository<RolePermission>().Query()
                .Where(rp => rp.RoleId == roleId && rp.IsGranted)
                .Include(rp => rp.Permission)
                .Select(rp => rp.Permission)
                .ToListAsync();

            return rolePermissions;
        }

        public async Task<List<Permission>> GetAllPermissionsAsync()
        {
            return await _unitOfWork.GetReadRepository<Permission>()
                .Query()
                .Where(p => p.IsActive)
                .OrderBy(p => p.Category)
                .ThenBy(p => p.SortOrder)
                .ToListAsync();
        }

        public async Task SyncPermissionsAsync()
        {
            var permissionType = typeof(PermissionConstants);
            var fields = permissionType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string));

            var existingPermissions = await _unitOfWork.GetReadRepository<Permission>()
                .Query()
                .ToDictionaryAsync(p => p.Name);

            foreach (var field in fields)
            {
                var permissionName = field.GetValue(null)?.ToString();
                if (string.IsNullOrEmpty(permissionName))
                    continue;

                if (!existingPermissions.ContainsKey(permissionName))
                {
                    var category = GetPermissionCategory(permissionName);
                    var displayName = GetPermissionDisplayName(permissionName);

                    var permission = new Permission
                    {
                        Name = permissionName,
                        DisplayName = displayName,
                        Category = category,
                        Description = $"Permission for {displayName}",
                        IsActive = true
                    };

                    await _unitOfWork.GetWriteRepository<Permission>().AddAsync(permission);
                    await _unitOfWork.SaveChangesAsync();
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }

        private void ClearUserPermissionCache(Guid roleId)
        {
            // Role'e sahip tüm kullanıcıların cache'ini temizle
            var userIds = _unitOfWork.UserRoles
                .Query()
                .Where(ur => ur.RoleId == roleId && ur.IsActive)
                .Select(ur => ur.UserId)
                .ToList();

            foreach (var userId in userIds)
            {
                var cacheKey = string.Format(PERMISSION_CACHE_KEY, userId);
                _cache.Remove(cacheKey);
            }
        }

        private string GetPermissionCategory(string permissionName)
        {
            var textInfo = CultureInfo.CurrentCulture.TextInfo;
            var parts = permissionName.Split('.');
            return parts.Length > 0 ? textInfo.ToTitleCase(parts[0]) : "General";
        }

        private string GetPermissionDisplayName(string permissionName)
        {
            var textInfo = CultureInfo.CurrentCulture.TextInfo;
            return permissionName
                .Replace(".", " ")
                .Split(' ')
                .Select(word => textInfo.ToTitleCase(word))
                .Aggregate((current, next) => current + " " + next);
        }
    }
}
 