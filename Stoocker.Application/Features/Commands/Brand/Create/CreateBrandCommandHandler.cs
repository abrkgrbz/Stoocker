using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Stoocker.Application.DTOs.Brand.Request;
using Stoocker.Application.DTOs.Brand.Response;
using Stoocker.Application.DTOs.Common;
using Stoocker.Application.DTOs.User.Request;
using Stoocker.Application.DTOs.User.Response;
using Stoocker.Application.Interfaces.Repositories;
using Stoocker.Application.Interfaces.Repositories.Entities.Brand;

namespace Stoocker.Application.Features.Commands.Brand.Create
{
    public class CreateBrandCommandHandler:IRequestHandler<CreateBrandCommand,Result<CreateBrandResponse>>
    {
        private readonly IMapper _mapper;
        private readonly IBrandWriteRepository _brandWriteRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateBrandRequest> _createValidator;
        private readonly ILogger<CreateBrandCommandHandler> _logger;
        public CreateBrandCommandHandler(IMapper mapper, IBrandWriteRepository brandWriteRepository, IUnitOfWork unitOfWork, IValidator<CreateBrandRequest> createValidator, ILogger<CreateBrandCommandHandler> logger)
        {
            _mapper = mapper;
            _brandWriteRepository = brandWriteRepository;
            _unitOfWork = unitOfWork;
            _createValidator = createValidator;
            _logger = logger;
        }

        public async Task<Result<CreateBrandResponse>> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
        {
             
            try
            {
                var requestDTO = _mapper.Map<CreateBrandRequest>(request);
                var validationResult = await _createValidator.ValidateAsync(requestDTO, cancellationToken);
                if (!validationResult.IsValid)
                {
                    return Result<CreateBrandResponse>.Failure(validationResult.Errors.First().ErrorMessage);
                }
                var brand = _mapper.Map<Domain.Entities.Brand>(requestDTO);

                // UnitOfWork üzerinden repository al
                var brandRepository = _unitOfWork.GetWriteRepository<Domain.Entities.Brand>();

                await brandRepository.AddAsync(brand, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Response DTO döndür
                var response = _mapper.Map<CreateBrandResponse>(brand);
                return Result<CreateBrandResponse>.Success(response); 
            }
            catch (Exception e)
            {
                 return Result<CreateBrandResponse>.Failure(e.Message);
            }
           
        }
    }
}
