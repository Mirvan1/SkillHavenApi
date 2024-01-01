using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
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

namespace SkillHaven.Application.Features.Blogs.Queries
{
    public class GetBlogQueryHandler : IRequestHandler<GetBlogQuery, GetBlogsDto>
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public readonly IStringLocalizer _localizer;
        public readonly IUtilService _utilService;
        private readonly IBlogVoteRepository _blogVoteRepository;

        public GetBlogQueryHandler(IHttpContextAccessor httpContextAccessor, IUserService userService, IMapper mapper, IBlogRepository blogRepository, IUserRepository userRepository, IUtilService utilService, IBlogVoteRepository blogVoteRepository)
        {
            _httpContextAccessor=httpContextAccessor;
            _userService=userService;
            _mapper=mapper;
            _blogRepository=blogRepository;
            _localizer=new Localizer();
            _userRepository=userRepository;
            _utilService=utilService;
            _blogVoteRepository=blogVoteRepository;
        }
        public async Task<GetBlogsDto> Handle(GetBlogQuery request, CancellationToken cancellationToken)
        {
            if (!_userService.isUserAuthenticated()) throw new UserVerifyException(_localizer["UnAuthorized", "Errors"].Value);

            var blog = await _blogRepository.GetByIdAsync(request.Id, cancellationToken);

            if (blog is null) throw new DatabaseValidationException(_localizer["NotFound", "Errors", "Blog "].Value);

            
            var blogginMap = _mapper.Map<GetBlogsDto>(blog);

            if(blog.UserId !=null)
            {
                var user =await _userRepository.GetByIdAsync(blog.UserId, cancellationToken);
                blogginMap.FullName =$"{user.FirstName} {user.LastName}";
                blogginMap.PhotoPath=_utilService.GetPhotoAsBase64(blogginMap?.PhotoPath);
                blogginMap.Vote= await _blogVoteRepository.VotesByBlog(blog.BlogId,cancellationToken);
            }            
            if (blog?.BlogComments != null) blogginMap.BlogComments= blog.BlogComments.Count();
            
            if (blog?.NOfReading is null) blog.NOfReading=1;
            else blog.NOfReading+=1;
            
            _blogRepository.Update(blog);
            await _blogRepository.SaveChangesAsync(cancellationToken);

            return blogginMap;
        }
    }
}
