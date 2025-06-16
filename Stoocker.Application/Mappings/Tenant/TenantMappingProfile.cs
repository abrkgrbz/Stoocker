using Stoocker.Application.DTOs.Tenant.Request;
using Stoocker.Application.DTOs.Tenant.Response;
using Stoocker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Stoocker.Application.DTOs.Common;
using TenantStatus = Stoocker.Application.DTOs.Tenant.Response.TenantStatus;

namespace Stoocker.Application.Mappings.Tenant
{
    public class TenantMappingProfile:Profile
    {
        public TenantMappingProfile()
        {
            // Tenant Entity -> Response DTOs
            CreateMap<Domain.Entities.Tenant, TenantResponse>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => GetTenantStatus(src))).ReverseMap();

            CreateMap<Domain.Entities.Tenant, TenantDetailResponse>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => GetTenantStatus(src)))
                .ForMember(dest => dest.UserCount, opt => opt.Ignore())
                .ForMember(dest => dest.RoleCount, opt => opt.Ignore())
                .ForMember(dest => dest.ActiveUserCount, opt => opt.Ignore())
                .ForMember(dest => dest.LastLoginAt, opt => opt.Ignore())
                .ForMember(dest => dest.RecentActivities, opt => opt.Ignore())
                .ForMember(dest => dest.Settings, opt => opt.MapFrom(src => GetDefaultTenantSettings()))
                .ForMember(dest => dest.CanBeDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.SuspensionReason, opt => opt.Ignore())
                .ForMember(dest => dest.SuspendedAt, opt => opt.Ignore()).ReverseMap();

            // Request DTOs -> Tenant Entity
            CreateMap<CreateTenantRequest, Domain.Entities.Tenant>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Users, opt => opt.Ignore())
                .ForMember(dest => dest.Roles, opt => opt.Ignore()).ReverseMap();

            CreateMap<UpdateTenantRequest, Domain.Entities.Tenant>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Users, opt => opt.Ignore())
                .ForMember(dest => dest.Roles, opt => opt.Ignore())
                .ForMember(dest => dest.Domain, opt => opt.Ignore()).ReverseMap(); // Domain separately handled

            CreateMap<PagedResult<TenantResponse>, PagedResult<Domain.Entities.Tenant>>().ReverseMap();
        }

        private static TenantStatus GetTenantStatus(Domain.Entities.Tenant tenant)
        {
            if (!tenant.IsActive)
                return TenantStatus.Inactive;

            return TenantStatus.Active;
        }

        private static TenantSettings GetDefaultTenantSettings()
        {
            return new TenantSettings
            {
                DefaultTimeZone = "Europe/Istanbul",
                DefaultLanguage = "tr-TR",
                Currency = "TRY",
                CurrencySymbol = "₺",
                PrimaryColor = "#3498db",
                SecondaryColor = "#2c3e50"
            };
        }
    }
    
}
