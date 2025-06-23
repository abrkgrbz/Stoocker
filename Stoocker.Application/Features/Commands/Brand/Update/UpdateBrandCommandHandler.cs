using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Stoocker.Application.DTOs.Common;
using Stoocker.Application.Interfaces.Repositories;

namespace Stoocker.Application.Features.Commands.Brand.Update
{
    public class UpdateBrandCommandHandler:IRequestHandler<UpdateBrandCommand,Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateBrandCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
        {

            throw new NotImplementedException();
        }
    }
}
