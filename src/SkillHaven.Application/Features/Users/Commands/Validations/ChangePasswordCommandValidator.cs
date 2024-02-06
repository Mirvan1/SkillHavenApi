using FluentValidation;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Localization;
using SkillHaven.Application.Configurations;
using SkillHaven.Application.Interfaces.Services;
using SkillHaven.Shared.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Features.Users.Commands.Validations
{
    public class ChangePasswordCommandValidator:AbstractValidator<ChangePasswordCommand>
    {
        private readonly IStringLocalizer _localizer;

        private readonly IUtilService _utilService;

        public ChangePasswordCommandValidator(IUtilService utilService)
        {
            _localizer=new Localizer();
            _utilService=utilService;

            RuleFor(p => p.OldPassword)
             .NotNull().WithMessage(_localizer["NotNull", "Errors"].Value)
           .MinimumLength(5)
           .MaximumLength(25)
           .WithMessage(_localizer["RangeError", "Errors", "5", "25"].Value);

            RuleFor(p => p.NewPassword)
             .NotNull().WithMessage(_localizer["NotNull", "Errors"].Value)
           .MinimumLength(5)
           .MaximumLength(25)
           .WithMessage(_localizer["RangeError", "Errors", "5", "25"].Value);           ;


            RuleFor(p => p.ConfirmPassword)
             .NotNull().WithMessage(_localizer["NotNull", "Errors"].Value)
           .MinimumLength(5)
           .MaximumLength(25)
           .WithMessage(_localizer["RangeError", "Errors", "5", "25"].Value);


            RuleFor(p => new { p.NewPassword, p.ConfirmPassword })
                .Must(x =>_utilService.isPasswordEqual(x.NewPassword, x.ConfirmPassword));
        }


    
    }
}
