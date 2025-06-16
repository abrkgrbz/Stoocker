using AutoMapper;
using Stoocker.Application.DTOs.Tenant.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.Mappings.Tenant
{
    public class TenantStatsMappingProfile : Profile
    {
        public TenantStatsMappingProfile()
        {
            // Tenant -> TenantStatsResponse
            CreateMap<Domain.Entities.Tenant, TenantStatsResponse>()
                .ForMember(dest => dest.TenantId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TenantName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.DaysSinceCreation, opt => opt.MapFrom(src => (DateTime.UtcNow - src.CreatedAt).Days))
                .ForMember(dest => dest.TotalUsers, opt => opt.Ignore())
                .ForMember(dest => dest.ActiveUsers, opt => opt.Ignore())
                .ForMember(dest => dest.InactiveUsers, opt => opt.Ignore())
                .ForMember(dest => dest.NewUsersThisMonth, opt => opt.Ignore())
                .ForMember(dest => dest.TotalRoles, opt => opt.Ignore())
                .ForMember(dest => dest.ActiveRoles, opt => opt.Ignore())
                .ForMember(dest => dest.LastLoginAt, opt => opt.Ignore())
                .ForMember(dest => dest.LastUserCreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.LoginsThisMonth, opt => opt.Ignore())
                .ForMember(dest => dest.MonthlyUserGrowth, opt => opt.Ignore())
                .ForMember(dest => dest.MonthlyLoginStats, opt => opt.Ignore());
        }
    }
}
