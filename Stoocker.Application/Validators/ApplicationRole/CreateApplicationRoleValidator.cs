using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Stoocker.Application.DTOs.Role.Request;

namespace Stoocker.Application.Validators.ApplicationRole
{
    public class CreateApplicationRoleValidator:AbstractValidator<CreateRoleRequest>
    {
        public CreateApplicationRoleValidator()
        {
            
        }
    }
}
