using FluentValidation;
using Microsoft.Extensions.Localization;
using SkillHaven.Application.Configurations;
using SkillHaven.Shared.Blog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Features.Blogs.Command.Validations
{
    public class CreateBlogCommandValidator:AbstractValidator<CreateBlogCommand>
    {
        private readonly IStringLocalizer _localizer;


        public CreateBlogCommandValidator()
        {
            _localizer=new Localizer();

            RuleFor(p => p.Title)
                .NotNull().WithMessage(_localizer["NotNull", "Errors"].Value)
                .MinimumLength(5)
                .MaximumLength(50)
                .WithMessage(_localizer["RangeError", "Errors", "5", "50"].Value);

            RuleFor(p=>p.Content)
                  .NotNull().WithMessage(_localizer["NotNull", "Errors"].Value)
                .MinimumLength(5)
                .MaximumLength(1000)
                .WithMessage(_localizer["RangeError", "Errors", "5", "1000"].Value);

            RuleFor(p => p.isPublished)
                .NotNull().WithMessage(_localizer["NotNull", "Errors"].Value);

        }
    }
}
