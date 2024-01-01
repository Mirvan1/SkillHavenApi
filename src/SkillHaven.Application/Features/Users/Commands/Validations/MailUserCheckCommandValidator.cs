﻿using FluentValidation;
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
    internal class MailUserCheckCommandValidator : AbstractValidator<MailUserCheckerCommand>
    {
        private readonly IStringLocalizer _localizer;
        public MailUserCheckCommandValidator()
        {
            _localizer=new Localizer();

            RuleFor(p => p.Email)
              .NotNull().WithMessage(_localizer["NotNull", "Errors"].Value)
              .EmailAddress()
              .WithMessage("A valid email address required");
        }
    }
}
