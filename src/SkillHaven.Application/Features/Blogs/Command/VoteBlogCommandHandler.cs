using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using RazorEngineCore;
using SkillHaven.Application.Configurations;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Application.Interfaces.Services;
using SkillHaven.Domain.Entities;
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
    internal class VoteBlogCommandHandler : IRequestHandler<VoteBlogCommand, int>
    {
        private readonly IBlogRepository _blogRepository;

        private readonly IBlogCommentRepository _blogCommentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public readonly IStringLocalizer _localizer;
        private readonly IBlogVoteRepository _blogVoteRepository;
        public VoteBlogCommandHandler(IHttpContextAccessor httpContextAccessor, IUserService userService, IMapper mapper, IBlogRepository blogRepository, IBlogCommentRepository blogCommentRepository, IBlogVoteRepository blogVoteRepository)
        {
            _httpContextAccessor=httpContextAccessor;
            _userService=userService;
            _mapper=mapper;
            _blogRepository=blogRepository;
            _blogCommentRepository=blogCommentRepository;
            _localizer=new Localizer();
            _blogVoteRepository=blogVoteRepository;
        }
        public Task<int> Handle(VoteBlogCommand request, CancellationToken cancellationToken)
        {
            if (!_userService.isUserAuthenticated()) throw new UserVerifyException(_localizer["UnAuthorized", "Errors"].Value);

            var blog = _blogRepository.GetById(request.BlogId);

            if (blog is null) throw new DatabaseValidationException(_localizer["NotFound", "Errors", "Blog"].Value);

            var getUser = _userService.GetUser();
            //if (blog.Vote is null) blog.Vote=0;

            //if (request.isIncreased) blog.Vote+=1;
            //else blog.Vote-=1;

            //_blogRepository.Update(blog);
            //_blogRepository.SaveChanges();

            //return Task.FromResult((int)blog.Vote);
            if (getUser is null) throw new DatabaseValidationException(_localizer["NotFound", "Errors", "User"].Value);

            var userVoted = _blogVoteRepository.GetByUserId(getUser.UserId,blog.BlogId);

            if (userVoted is not null && userVoted?.Count >0) throw new UserVerifyException("You already give vote");

            _blogVoteRepository.Add(new BlogVote()
            {
                BlogId=blog.BlogId,
                UserId=_userService.GetUser().UserId,
                VoteValue=request.isIncreased
            });

            _blogVoteRepository.SaveChanges();

            return Task.FromResult(_blogVoteRepository.VotesByBlog(blog.BlogId));

        }
    }
}
