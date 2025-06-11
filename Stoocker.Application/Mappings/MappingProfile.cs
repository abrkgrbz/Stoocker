using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Stoocker.Application.DTOs.Brand.Request;
using Stoocker.Application.Features.Commands.Brand;
using Stoocker.Domain.Entities;

namespace Stoocker.Application.Mappings
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateBrandRequest, CreateBrandCommand>().ReverseMap();
            CreateMap<Brand, CreateBrandRequest>().ReverseMap();
        }
    }
}
