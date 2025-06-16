using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Stoocker.Application.DTOs.Brand.Request;
using Stoocker.Application.DTOs.Brand.Response;
using Stoocker.Application.Features.Commands.Brand.Create;

namespace Stoocker.Application.Mappings.Brand
{
    public class BrandMappingProfile:Profile
    {
        public BrandMappingProfile()
        {
            CreateMap<CreateBrandRequest, CreateBrandCommand>().ReverseMap();
            CreateMap<Domain.Entities.Brand, CreateBrandRequest>().ReverseMap();
            CreateMap<Domain.Entities.Brand, GetBrandResponse>().ReverseMap();
        }
    }
}
