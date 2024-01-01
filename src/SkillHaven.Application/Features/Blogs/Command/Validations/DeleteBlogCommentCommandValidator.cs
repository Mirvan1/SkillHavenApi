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
    public class DeleteBlogCommentCommandValidator:AbstractValidator<DeleteBlogCommand>
    {
        private readonly IStringLocalizer _localizer;

        public DeleteBlogCommentCommandValidator()
        {
            _localizer=new Localizer();

                RuleFor(p => p.Id)
             .NotNull().WithMessage(_localizer["NotNull", "Errors"].Value);

        }
    }
}
