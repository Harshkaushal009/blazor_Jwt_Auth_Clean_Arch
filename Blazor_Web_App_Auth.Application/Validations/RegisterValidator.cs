using Blazor_Web_App_Auth.Domain.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor_Web_App_Auth.Application.Validations
{
    public class RegisterValidator:AbstractValidator<User>
    {
        public RegisterValidator()
        {
            RuleFor(u => u.Name).
                NotEmpty().WithMessage("UserName Is A Require Feild !")
                .MinimumLength(6).WithMessage("Oh No ! User Name Less Than 6 chracters");
            RuleFor(u => u.Email)
               .NotEmpty()
                .WithMessage("Email Should Be Filled Correctly")
                .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible);
            RuleFor(u => u.PasswordHash).
                NotEmpty().WithMessage("Oh No! You Forgot To Enter Password")
                .MinimumLength(6).WithMessage("Use Strong Password !");




        }
    }
}
