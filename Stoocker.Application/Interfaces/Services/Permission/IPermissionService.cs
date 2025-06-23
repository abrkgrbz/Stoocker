using Stoocker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.Interfaces.Services.Permission
{
    public interface IPermissionService
    {
        Task<bool> HasPermissionAsync(Guid userId, string permission);
        Task<bool> HasPermissionsAsync(Guid userId, params string[] permissions);
        Task<bool> HasAnyPermissionAsync(Guid userId, params string[] permissions);
        Task<List<string>> GetUserPermissionsAsync(Guid userId);
        Task<Dictionary<string, bool>> CheckPermissionsAsync(Guid userId, params string[] permissions);
        Task GrantPermissionToRoleAsync(Guid roleId, string permission);
        Task RevokePermissionFromRoleAsync(Guid roleId, string permission);
        Task<List<Domain.Entities.Permission>> GetRolePermissionsAsync(Guid roleId);
        Task<List<Domain.Entities.Permission>> GetAllPermissionsAsync();
        Task SyncPermissionsAsync(); // Database'e tanımlı permission'ları ekler
    }
}
