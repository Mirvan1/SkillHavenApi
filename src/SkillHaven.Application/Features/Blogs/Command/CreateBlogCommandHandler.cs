﻿using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using SkillHaven.Application.Configurations;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Application.Interfaces.Services;
using SkillHaven.Domain.Entities;
using SkillHaven.Shared.Blog;
using SkillHaven.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Features.Blogs.Command
{
    public class CreateBlogCommandHandler : IRequestHandler<CreateBlogCommand, bool>
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public readonly IStringLocalizer _localizer;

        public CreateBlogCommandHandler(IHttpContextAccessor httpContextAccessor, IUserService userService, IMapper mapper, IBlogRepository blogRepository)
        {
            _httpContextAccessor=httpContextAccessor;
            _userService=userService;
            _mapper=mapper;
            _blogRepository=blogRepository;
            _localizer=new Localizer();
        }
        public Task<bool> Handle(CreateBlogCommand request, CancellationToken cancellationToken)
        {

            if (!_userService.isUserAuthenticated()) throw new UserVerifyException(_localizer["UnAuthorized", "Errors"].Value);
            
            var addBlog = new Blog()
            {
                Content=request.Content,
                Title=request.Title,
                IsPublished=request.isPublished,
                //PublishDate=default,
                UserId=_userService.GetUser().UserId,
                //User=_mapper.Map<User>(_userService.GetUser())
            };


            _blogRepository.Add(addBlog);
           int result= _blogRepository.SaveChanges();
            return Task.FromResult(result>0);
        }
    }
}
