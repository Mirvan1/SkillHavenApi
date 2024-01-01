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
    public class VoteBlogCommandValidator : AbstractValidator<VoteBlogCommand>
    {
        private readonly IStringLocalizer _localizer;
        public VoteBlogCommandValidator()
        {
            _localizer=new Localizer();

            RuleFor(x => x.BlogId)
                                .NotNull().WithMessage(_localizer["NotNull", "Errors"].Value);

            RuleFor(x => x.isIncreased)
                                .NotNull().WithMessage(_localizer["NotNull", "Errors"].Value);

        }
    }
}
