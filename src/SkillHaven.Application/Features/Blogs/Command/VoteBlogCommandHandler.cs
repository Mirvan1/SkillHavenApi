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
    internal class VoteBlogCommandHandler : IRequestHandler<VoteBlogCommand, int>
    {
        private readonly IBlogRepository _blogRepository;

        private readonly IBlogCommentRepository _blogCommentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public VoteBlogCommandHandler(IHttpContextAccessor httpContextAccessor, IUserService userService, IMapper mapper, IBlogRepository blogRepository, IBlogCommentRepository blogCommentRepository)
        {
            _httpContextAccessor=httpContextAccessor;
            _userService=userService;
            _mapper=mapper;
            _blogRepository=blogRepository;
            _blogCommentRepository=blogCommentRepository;
        }
        public Task<int> Handle(VoteBlogCommand request, CancellationToken cancellationToken)
        {
            // if (!_userService.isUserAuthenticated()) throw new UserVerifyException("User is not authorize");

            var blog = _blogRepository.GetById(request.BlogId);

            if (blog is null) throw new DatabaseValidationException("Blog not found");

            if (blog.Vote is null) blog.Vote=0;
            //else
            //{
                if (request.isIncreased) blog.Vote+=1;
                else blog.Vote-=1;
          //  }
            _blogRepository.Update(blog);
            _blogRepository.SaveChanges();

            return Task.FromResult((int)blog.Vote);
        }
    }
}
