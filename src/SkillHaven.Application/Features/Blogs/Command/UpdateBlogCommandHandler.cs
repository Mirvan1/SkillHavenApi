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
    internal class UpdateBlogCommandHandler : IRequestHandler<UpdateBlogCommand, bool>
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public UpdateBlogCommandHandler(IHttpContextAccessor httpContextAccessor, IUserService userService, IMapper mapper, IBlogRepository blogRepository)
        {
            _httpContextAccessor=httpContextAccessor;
            _userService=userService;
            _mapper=mapper;
            _blogRepository=blogRepository;
        }
        public Task<bool> Handle(UpdateBlogCommand request, CancellationToken cancellationToken)
        {
            //if (!_userService.isUserAuthenticated()) throw new UserVerifyException("User is not authorize");
            var blog = _blogRepository.GetById(request.Id);

            if (blog is null) throw new DatabaseValidationException("Blog not found");

            blog.Title??=request.Title;
            blog.Content??=request.Content;
            blog.UpdateDate=DateTime.Now;
            blog.IsPublished=request.isPublished;

            _blogRepository.Update(blog);
           int result= _blogRepository.SaveChanges();
            return Task.FromResult(result>0);
        }
    }
}
