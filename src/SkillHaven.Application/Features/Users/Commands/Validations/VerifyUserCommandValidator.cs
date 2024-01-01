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
    public class VerifyUserCommandValidator:AbstractValidator<VerifyUserCommand>
    {
        private readonly IStringLocalizer _localizer;

        public VerifyUserCommandValidator()
        {
            _localizer=new Localizer();
            RuleFor(p => p.MailSendCode)
                     .NotNull().WithMessage(_localizer["NotNull", "Errors"].Value)
                   .MinimumLength(1)
                   .MaximumLength(6)
                   .WithMessage(_localizer["RangeError", "Errors", "1", "6"].Value);


        }
    }
}
