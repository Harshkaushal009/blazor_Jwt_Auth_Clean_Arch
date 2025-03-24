using Blazor_Web_App_Auth.Domain.ViewModels;
using FluentValidation;

namespace Blazor_Web_App_Auth.Application.Validations
{
    public class LoginValidator:AbstractValidator<LoginViewModel>
    {
        public LoginValidator()
        {
            RuleFor(login => login.Email).NotEmpty()
                .WithMessage("Email Should Be Filled Correctly")
                .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible);

            RuleFor(login => login.Password).NotEmpty().
                WithMessage("Password Should Be Filled Correctly").Must(password => password.Length >= 6);


        }
    }
}
