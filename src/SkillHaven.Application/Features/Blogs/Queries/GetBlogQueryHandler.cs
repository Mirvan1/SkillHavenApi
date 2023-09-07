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

namespace SkillHaven.Application.Features.Blogs.Queries
{
    public class GetBlogQueryHandler : IRequestHandler<GetBlogQuery, GetBlogsDto>
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public GetBlogQueryHandler(IHttpContextAccessor httpContextAccessor, IUserService userService, IMapper mapper, IBlogRepository blogRepository)
        {
            _httpContextAccessor=httpContextAccessor;
            _userService=userService;
            _mapper=mapper;
            _blogRepository=blogRepository;
        }
        public Task<GetBlogsDto> Handle(GetBlogQuery request, CancellationToken cancellationToken)
        {
            //if (!_userService.isUserAuthenticated()) throw new UserVerifyException("User is not authorize");
            var blog = _blogRepository.GetById(request.Id);

            if (blog is null) throw new DatabaseValidationException("Blog not found");

            var blogginMap = _mapper.Map<GetBlogsDto>(blog);

            return Task.FromResult(blogginMap);
        }
    }
}
