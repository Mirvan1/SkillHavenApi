using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using SkillHaven.Application.Configurations;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Application.Interfaces.Services;
using SkillHaven.Shared;
using SkillHaven.Shared.Blog;
using SkillHaven.Shared.Exceptions;
using SkillHaven.Shared.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Features.Blogs.Queries
{
    internal class BlogByCommentsQueryHandler : IRequestHandler<BlogByCommentsQuery, PaginatedResult<BlogCommentDto>>
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IBlogCommentRepository _blogCommentRepository;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public readonly IStringLocalizer _localizer;
        public BlogByCommentsQueryHandler(IHttpContextAccessor httpContextAccessor, IUserService userService, IMapper mapper, IBlogRepository blogRepository, IBlogCommentRepository blogCommentRepository)
        {
            _httpContextAccessor=httpContextAccessor;
            _userService=userService;
            _mapper=mapper;
            _blogRepository=blogRepository;
            _blogCommentRepository=blogCommentRepository;
            _localizer=new Localizer();
        }

        public Task<PaginatedResult<BlogCommentDto>> Handle(BlogByCommentsQuery request, CancellationToken cancellationToken)
        {
            if (!_userService.isUserAuthenticated()) throw new UserVerifyException(_localizer["UnAuthorized", "Errors"].Value);

            var blogComments = _blogCommentRepository.getCommentsByBlog(request.BlogId, request.Page, request.PageSize, request.OrderBy, request.OrderByPropertname);

            if (blogComments is null) throw new DatabaseValidationException(_localizer["NotFound", "Errors", "Blog Comment"].Value);

            PaginatedResult<BlogCommentDto> result = new()
            {
                TotalCount=blogComments.TotalCount,
                TotalPages=blogComments.TotalPages,
            };
            if (blogComments.Data is not null)
            {
                foreach (var blogComment in blogComments.Data)
                {
                    var blogCommentDto = new BlogCommentDto()
                    {
                        BlogCommentsId=blogComment.BlogCommentsId,
                        BlogId=blogComment.Blog.BlogId,
                        UserId=blogComment.User.UserId,
                        CommentContent=blogComment.CommentContent,
                        CommentTitle=blogComment.CommentTitle,
                        PublishDate=blogComment.PublishDate,
                        isPublished=blogComment.isPublished,
                        FullName=blogComment.User.FirstName + " "+ blogComment.User.LastName,
                        BlogName=blogComment.Blog.Title
                    };
                    result.Data.Add(blogCommentDto);
                }
            }
            return Task.FromResult(result);
        }
    }
}
