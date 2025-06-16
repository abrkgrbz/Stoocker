using AutoMapper;
using Stoocker.Application.DTOs.User.Request;
using Stoocker.Application.DTOs.User.Response;
using Stoocker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.Mappings.User
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        { 
            CreateMap<ApplicationUser, UserResponse>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src =>
                    src.UserRoles.Where(ur => ur.IsActive).Select(ur => ur.Role.Name)));

            CreateMap<ApplicationUser, UserDetailResponse>()
                .ForMember(dest => dest.TenantName, opt => opt.MapFrom(src => src.Tenant.Name))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src =>
                    src.UserRoles.Where(ur => ur.IsActive).Select(ur => new UserDetailResponse.RoleInfo
                    {
                        Id = ur.Role.Id,
                        Name = ur.Role.Name,
                        Description = ur.Role.Description,
                        AssignedAt = ur.AssignedAt
                    })))
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src =>
                    GetUserPermissions(src)));

            // Request to Entity
            CreateMap<CreateUserRequest, ApplicationUser>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.NormalizedUserName, opt => opt.MapFrom(src => src.Email.ToUpperInvariant()))
                .ForMember(dest => dest.NormalizedEmail, opt => opt.MapFrom(src => src.Email.ToUpperInvariant()));

            CreateMap<UpdateUserRequest, ApplicationUser>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.Ignore());
        }

        private List<string> GetUserPermissions(ApplicationUser user)
        {
            return user.UserRoles
                .Where(ur => ur.IsActive && ur.Role.IsActive)
                .SelectMany(ur =>
                {
                    if (string.IsNullOrEmpty(ur.Role.Permissions))
                        return new List<string>();

                    return System.Text.Json.JsonSerializer.Deserialize<List<string>>(ur.Role.Permissions) ?? new List<string>();
                })
                .Distinct()
                .ToList();
        }
    }
}
