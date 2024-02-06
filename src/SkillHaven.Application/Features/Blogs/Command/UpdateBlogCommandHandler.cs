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
using SkillHaven.Shared.UtilDtos;
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
        public readonly IStringLocalizer _localizer;
        public readonly IUtilService _utilService;
        public UpdateBlogCommandHandler(IHttpContextAccessor httpContextAccessor, IUserService userService, IMapper mapper, IBlogRepository blogRepository, IUtilService utilService)
        {
            _httpContextAccessor=httpContextAccessor;
            _userService=userService;
            _mapper=mapper;
            _blogRepository=blogRepository;
            _localizer=new Localizer();
            _utilService=utilService;
        }
        public async Task<bool> Handle(UpdateBlogCommand request, CancellationToken cancellationToken)
        {
            if (!_userService.isUserAuthenticated()) throw new UserVerifyException(_localizer["UnAuthorized", "Errors"].Value);
            
            var blog = await _blogRepository.GetByIdAsync(request.Id,cancellationToken);

            if (blog is null) throw new DatabaseValidationException(_localizer["NotFound", "Errors", "Blog"].Value);
            blog.UpdateDate=DateTime.Now;

            if(!string.IsNullOrEmpty(request.Title))  blog.Title=request.Title;
            if (!string.IsNullOrEmpty(request.Content))  blog.Content=request.Content;
            if(request?.isPublished!=null) blog.IsPublished=request.isPublished;
            if(!string.IsNullOrEmpty(request.Photo)) blog.PhotoPath=_utilService.SavePhoto(request?.Photo, PhotoTypes.BlogPhoto.ToString()+"_"+ request.Title+DateTime.Now.ToLongDateString());
            if (request.BlogTopicId!=null) blog.BlogTopicId=request.BlogTopicId;

            _blogRepository.Update(blog);
            int result = await _blogRepository.SaveChangesAsync(cancellationToken);
            return result>0;
        }
    }
}
