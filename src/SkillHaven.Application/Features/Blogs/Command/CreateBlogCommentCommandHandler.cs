using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Application.Interfaces.Services;
using SkillHaven.Domain.Entities;
using SkillHaven.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Features.Blogs.Command
{
    internal class CreateBlogCommentCommandHandler : IRequestHandler<CreateBlogCommentCommand, bool>
    {
        private readonly IBlogRepository _blogRepository;

        private readonly IBlogCommentRepository _blogCommentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public CreateBlogCommentCommandHandler(IHttpContextAccessor httpContextAccessor, IUserService userService, IMapper mapper, IBlogRepository blogRepository, IBlogCommentRepository blogCommentRepository)
        {
            _httpContextAccessor=httpContextAccessor;
            _userService=userService;
            _mapper=mapper;
            _blogRepository=blogRepository;
            _blogCommentRepository=blogCommentRepository;
        }
        public Task<bool> Handle(CreateBlogCommentCommand request, CancellationToken cancellationToken)
        {
            // if (!_userService.isUserAuthenticated()) throw new UserVerifyException("User is not authorize");

            var addComment = new BlogComments()
            {
                BlogId=request.BlogId,
                UserId=_userService.GetUser().UserId,
                CommentContent=request.CommentContent,
                CommentTitle=request.CommentTitle,
                PublishDate=DateTime.Now,
                isPublished=true
            };
            _blogCommentRepository.Add(addComment);
            int result = _blogCommentRepository.SaveChanges();

            return Task.FromResult(result>0);
        }
    }
}
