using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Stoocker.Application.DTOs.Brand.Response;
using Stoocker.Application.DTOs.Common;
using Stoocker.Application.Interfaces.Repositories;
using Stoocker.Application.Interfaces.Repositories.Entities.Brand;

namespace Stoocker.Application.Features.Queries.Brand.GetBrands
{
    public class GetBrandsQueryHandler:IRequestHandler<GetBrandsQuery,PagedResult<GetBrandResponse>>
    {
        private readonly IMapper _mapper;
        private readonly IBrandReadRepository _brandRepository;
        private readonly IUnitOfWork _unitOfWork;
        public GetBrandsQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, IBrandReadRepository brandRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _brandRepository = brandRepository;
        }

        public async Task<PagedResult<GetBrandResponse>> Handle(GetBrandsQuery request, CancellationToken cancellationToken)
        {
            var data= await _brandRepository.GetBrandsByTenantIdAsync(
                tenantId: _unitOfWork.GetCurrentTenant() ?? Guid.Empty, 
                searchTerm: request.SearchTerm ?? string.Empty,
                page: request.PageNumber,
                pageSize: request.PageSize,
                cancellationToken: cancellationToken);
            var map= _mapper.Map<List<GetBrandResponse>>(data);
            return new PagedResult<GetBrandResponse>(
                items: map,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                totalCount: data.Count());
        }
    }
}
