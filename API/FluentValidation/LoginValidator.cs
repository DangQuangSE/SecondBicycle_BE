using Application.DTOs.Auth;
using FluentValidation;

namespace API.FluentValidation
{
    public class LoginValidator : AbstractValidator<LoginRequest>
    {
        public LoginValidator()
        {
            RuleFor(r => r.Email)
                .NotEmpty().WithMessage("Email is required.");
            RuleFor(r => r.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}
