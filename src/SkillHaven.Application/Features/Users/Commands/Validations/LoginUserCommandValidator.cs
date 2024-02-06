using FluentValidation;
using Microsoft.Extensions.Localization;
using SkillHaven.Application.Configurations;
using SkillHaven.Shared.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Features.Users.Commands.Validations
{
    public class LoginUserCommandValidator:AbstractValidator<LoginUserCommand>
    {
        private readonly IStringLocalizer _localizer;

        public LoginUserCommandValidator()
        {
            _localizer=new Localizer();


            RuleFor(p => p.Email)
               .NotNull().WithMessage(_localizer["NotNull", "Errors"].Value)
               .EmailAddress()
               .WithMessage("A valid email address required");

            RuleFor(p => p.Password)
             .NotNull().WithMessage(_localizer["NotNull", "Errors"].Value)
           .MinimumLength(5)
           .MaximumLength(25)
           .WithMessage(_localizer["RangeError", "Errors", "5", "25"].Value);
        }
    }
}
