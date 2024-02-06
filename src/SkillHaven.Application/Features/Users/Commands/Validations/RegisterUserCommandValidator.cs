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
    public class RegisterUserCommandValidator:AbstractValidator<RegisterUserCommand>
    {
        private readonly IStringLocalizer _localizer;

        public RegisterUserCommandValidator()
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


            RuleFor(p => p.Password)
            .NotNull().WithMessage(_localizer["NotNull", "Errors"].Value)
          .MinimumLength(5)
          .MaximumLength(25)
          .WithMessage(_localizer["RangeError", "Errors", "5", "25"].Value);

            RuleFor(p => p.Role)
            .NotNull().WithMessage(_localizer["NotNull", "Errors"].Value).IsInEnum();


            //RuleFor(p => p.SupervisorInfo)
            //    .Must(x => SupervisorValidation(x));

            //RuleFor(p => p.ConsultantInfo)
            //    .Must(x => ConsultantValidation(x));

        }

        private bool SupervisorValidation(SupervisorRegistrationInfo SupervisorInfo)
        {
            if (SupervisorInfo!=null)
            {
                if(SupervisorInfo.Expertise==null || string.IsNullOrEmpty(SupervisorInfo.Description))
                {
                    RuleFor(x=>x.SupervisorInfo.Expertise)
                    .NotNull().WithMessage(_localizer["NotNull", "Errors"].Value)
                    .MinimumLength(5)
                    .MaximumLength(50)
                    .WithMessage(_localizer["RangeError", "Errors", "5", "50"].Value);
                
                    RuleFor(x=>x.SupervisorInfo.Description)
                    .NotNull().WithMessage(_localizer["NotNull", "Errors"].Value)
                    .MinimumLength(5)
                    .MaximumLength(50)
                    .WithMessage(_localizer["RangeError", "Errors", "5", "50"].Value);

                    return false;
                }
            }

            return true;
        }



        private bool ConsultantValidation(ConsultantRegistrationInfo consultantInfo)
        {
            if (consultantInfo!=null)
            {
                if (consultantInfo?.Experience==null || string.IsNullOrEmpty(consultantInfo.Description))
                {
                    RuleFor(x => x.ConsultantInfo.Experience)
                    .NotNull().WithMessage(_localizer["NotNull", "Errors"].Value);


                    RuleFor(x => x.ConsultantInfo.Description)
                    .NotNull().WithMessage(_localizer["NotNull", "Errors"].Value)
                    .MinimumLength(5)
                    .MaximumLength(50)
                    .WithMessage(_localizer["RangeError", "Errors", "5", "50"].Value);

                    return false;
                }
            }

            return true;
        }



    }
}
