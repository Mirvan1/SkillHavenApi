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
    public class CreateBlogCommentCommandValidator : AbstractValidator<CreateBlogCommentCommand>
    {
        private readonly IStringLocalizer _localizer;

        public CreateBlogCommentCommandValidator()
        {
            _localizer=new Localizer();

            RuleFor(p => p.BlogId)
             .NotNull().WithMessage(_localizer["NotNull", "Errors"].Value);

            //RuleFor(p => p.CommentTitle)
            //      .NotNull().WithMessage(_localizer["NotNull", "Errors"].Value)
            //    .MinimumLength(5)
            //    .MaximumLength(100)
            //    .WithMessage(_localizer["RangeError", "Errors", "5", "100"].Value);

            RuleFor(p => p.CommentContent)
             .NotNull().WithMessage(_localizer["NotNull", "Errors"].Value)
            .MinimumLength(5)
            .MaximumLength(100)
            .WithMessage(_localizer["RangeError", "Errors", "5", "100"].Value);

            RuleFor(p => p.isPublished)
                .NotNull().WithMessage(_localizer["NotNull", "Errors"].Value);
        }
    }
}
