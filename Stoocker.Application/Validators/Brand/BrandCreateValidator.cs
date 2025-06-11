using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Stoocker.Application.DTOs.Brand.Request;

namespace Stoocker.Application.Validators.Brand
{
    public class BrandCreateValidator:AbstractValidator<CreateBrandRequest>
    {
        public BrandCreateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Brand Name is required")
                .NotNull().WithMessage("Brand Name is required")
                .MaximumLength(200).WithMessage("Brand Name cannot exceed 200 characters");

            RuleFor(x => x.Description) 
                .MaximumLength(500).WithMessage("Brand Description cannot exceed 500 characters");

            RuleFor(x => x.Website)
                .MaximumLength(300).WithMessage("Brand Website cannot exceed 300 characters");

            RuleFor(x => x.ContactEmail)
                .MaximumLength(200).WithMessage("Brand ContactEmail cannot exceed 200 characters");

            RuleFor(x => x.ContactPhone)
                .MaximumLength(20).WithMessage("Brand ContactPhone cannot exceed 20 characters");


        }
    }
}
