using FluentValidation;
using Microsoft.Extensions.Localization;
using SkillHaven.Application.Configurations;
using SkillHaven.Application.Interfaces.Services;
using SkillHaven.Shared.User.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Features.Users.Commands.Validations
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        private readonly IStringLocalizer _localizer;
        private readonly IUtilService _utilService;

        public ResetPasswordCommandValidator(IUtilService utilService)
        {
            _localizer=new Localizer();
            _utilService=utilService;

            RuleFor(p => p.Token)
                          .NotNull().WithMessage(_localizer["NotNull", "Errors"].Value);



            RuleFor(p => p.Email)
            .NotNull().WithMessage(_localizer["NotNull", "Errors"].Value)
            .EmailAddress()
            .WithMessage("A valid email address required");


            RuleFor(p => p.Password)
             .NotNull().WithMessage(_localizer["NotNull", "Errors"].Value)
           .MinimumLength(5)
           .MaximumLength(25)
           .WithMessage(_localizer["RangeError", "Errors", "5", "25"].Value); ;


            RuleFor(p => p.ConfirmPassword)
             .NotNull().WithMessage(_localizer["NotNull", "Errors"].Value)
           .MinimumLength(5)
           .MaximumLength(25)
           .WithMessage(_localizer["RangeError", "Errors", "5", "25"].Value);


            RuleFor(p => new { p.Password, p.ConfirmPassword })
                .Must(x =>_utilService.isPasswordEqual(x.Password, x.ConfirmPassword));

        }
    }
}
