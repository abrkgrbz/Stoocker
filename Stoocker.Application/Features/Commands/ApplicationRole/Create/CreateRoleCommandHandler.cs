using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Stoocker.Application.DTOs.Brand.Response;
using Stoocker.Application.DTOs.Common;
using Stoocker.Application.DTOs.Role.Request;
using Stoocker.Application.DTOs.Role.Response;
using Stoocker.Application.Interfaces.Repositories;
using Stoocker.Application.Validators.ApplicationRole;
using Stoocker.Domain.Entities;

namespace Stoocker.Application.Features.Commands.ApplicationRole.Create
{
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<RoleResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateRoleRequest> _createValidator;

        public CreateRoleCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, IValidator<CreateRoleRequest> createValidator)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _createValidator = createValidator;
        }

        public async Task<Result<RoleResponse>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var requestDTO = _mapper.Map<CreateRoleRequest>(request);
                var validationResult = await _createValidator.ValidateAsync(requestDTO, cancellationToken);
                if (!validationResult.IsValid)
                {
                    return Result<RoleResponse>.Failure(validationResult.Errors.First().ErrorMessage);
                }

                var role = _mapper.Map<Domain.Entities.ApplicationRole>(requestDTO);
                var roleRepository = _unitOfWork.Roles.AddAsync(role, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                var response = _mapper.Map<RoleResponse>(roleRepository);
                return Result<RoleResponse>.Success(response);
            }
            catch (Exception e)
            {
                return Result<RoleResponse>.Failure(e.Message);
            }
        }
    }
}
