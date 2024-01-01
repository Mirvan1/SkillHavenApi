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
    public class UpdateUserCommandValidator:AbstractValidator<UpdateUserCommand>
    {
        private readonly IStringLocalizer _localizer;
        public UpdateUserCommandValidator()
        {
            _localizer=new Localizer();


            RuleFor(p => p.FirstName)
              .NotNull().WithMessage(_localizer["NotNull", "Errors"].Value)
            .MinimumLength(5)
            .MaximumLength(50)
            .WithMessage(_localizer["RangeError", "Errors", "5", "50"].Value);


            RuleFor(p => p.LastName)
            .NotNull().WithMessage(_localizer["NotNull", "Errors"].Value)
          .MinimumLength(5)
          .MaximumLength(50)
          .WithMessage(_localizer["RangeError", "Errors", "5", "50"].Value);


            RuleFor(p => p.Email)
                .NotNull().WithMessage(_localizer["NotNull", "Errors"].Value)
                .EmailAddress()
                .WithMessage("A valid email address required");




            RuleFor(p => p.Role)
            .NotNull().WithMessage(_localizer["NotNull", "Errors"].Value);


 
        }


 
    }
}
