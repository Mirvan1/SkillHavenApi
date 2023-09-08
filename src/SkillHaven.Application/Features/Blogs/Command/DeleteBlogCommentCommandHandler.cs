using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Application.Interfaces.Services;
using SkillHaven.Shared;
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
        public DeleteBlogCommentCommandHandler(IHttpContextAccessor httpContextAccessor, IUserService userService, IMapper mapper, IBlogRepository blogRepository, IBlogCommentRepository blogCommentRepository)
        {
            _httpContextAccessor=httpContextAccessor;
            _userService=userService;
            _mapper=mapper;
            _blogRepository=blogRepository;
            _blogCommentRepository=blogCommentRepository;
        }
        public Task<bool> Handle(DeleteBlogCommentCommand request, CancellationToken cancellationToken)
        {
            var blogComment = _blogCommentRepository.GetById(request.BlogCommentId);
            if (blogComment is null) throw new DatabaseValidationException("Cannot find comment");

            blogComment.isPublished=false;

            _blogCommentRepository.Update(blogComment);
            int result=_blogCommentRepository.SaveChanges();

            return Task.FromResult(result>0);
        }
    }
}
