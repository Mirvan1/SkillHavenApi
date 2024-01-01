using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using SkillHaven.Application.Configurations;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Application.Interfaces.Services;
using SkillHaven.Shared.Blog;
using SkillHaven.Shared.Exceptions;
using SkillHaven.Shared.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Features.Blogs.Command
{
    public class DeleteBlogCommentCommandHandler : IRequestHandler<DeleteBlogCommentCommand, bool>
    {
        private readonly IBlogRepository _blogRepository;

        private readonly IBlogCommentRepository _blogCommentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public readonly IStringLocalizer _localizer;

        public DeleteBlogCommentCommandHandler(IHttpContextAccessor httpContextAccessor, IUserService userService, IMapper mapper, IBlogRepository blogRepository, IBlogCommentRepository blogCommentRepository)
        {
            _httpContextAccessor=httpContextAccessor;
            _userService=userService;
            _mapper=mapper;
            _blogRepository=blogRepository;
            _blogCommentRepository=blogCommentRepository;
            _localizer=new Localizer();
        }
        public async Task<bool> Handle(DeleteBlogCommentCommand request, CancellationToken cancellationToken)
        {

            if (!_userService.isUserAuthenticated()) throw new UserVerifyException(_localizer["UnAuthorized", "Errors"].Value);

            var blogComment = await _blogCommentRepository.GetByIdAsync(request.BlogCommentId,cancellationToken);
            if (blogComment is null) throw new DatabaseValidationException(_localizer["NotFound", "Errors","Comment"].Value);

            blogComment.isPublished=false;

            _blogCommentRepository.Update(blogComment);
            int result=await _blogCommentRepository.SaveChangesAsync(cancellationToken);

            return result>0;
        }
    }
}
