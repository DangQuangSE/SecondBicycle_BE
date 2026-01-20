using Application.DTOs.Users;
using FluentValidation;

namespace API.FluentValidation
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.RoleId)
                .GreaterThan(0).WithMessage("Invalid role");

            RuleFor(x => x.Status)
                .InclusiveBetween((byte)0, (byte)1)
                .WithMessage("Status must be 0 (Banned) or 1 (Active)");
        }
    }
}
