using Blazor_Web_App_Auth.Domain.Models;
using FluentValidation;


namespace Blazor_Web_App_Auth.Application.Validations
{
    public class ProductValidator:AbstractValidator<Product>
    {

        public ProductValidator()
        {
            RuleFor(product => product.Name)
                .NotEmpty().WithMessage("Product Name is required")
                .MinimumLength(3).WithMessage("Product Name must be at least 3 characters");


            RuleFor(product => product.Quantity)
    .NotEmpty().WithMessage("Product Quantity is required")
    .GreaterThan(0).WithMessage("Product Qty 0 or negative values are not allowed");


        }
    }
}
