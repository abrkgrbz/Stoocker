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

namespace Stoocker.Application.Features.Queries.Brand.GetBrand
{
    public class GetBrandQueryHandler:IRequestHandler<GetBrandQuery,Result<GetBrandResponse>>
    {
        private IBrandReadRepository _brandReadRepository; 
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetBrandQueryHandler(IBrandReadRepository brandReadRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _brandReadRepository = brandReadRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetBrandResponse>> Handle(GetBrandQuery request, CancellationToken cancellationToken)
        {
            var data = await _brandReadRepository.GetBrandsById(Id: request.Id,
                tenantId: _unitOfWork.GetCurrentTenant() ?? Guid.Empty);
            var map = _mapper.Map<GetBrandResponse>(data);
            return Result<GetBrandResponse>.Success(map);
        }
    }
}
