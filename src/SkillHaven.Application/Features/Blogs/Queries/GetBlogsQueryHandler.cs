﻿using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Application.Interfaces.Services;
using SkillHaven.Domain.Entities;
using SkillHaven.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Features.Blogs.Queries
{
    public class GetBlogsQueryHandler : IRequestHandler<GetBlogsQuery, PaginatedResult<GetBlogsDto>>
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public GetBlogsQueryHandler( IHttpContextAccessor httpContextAccessor, IUserService userService, IMapper mapper, IBlogRepository blogRepository)
        {
            _httpContextAccessor=httpContextAccessor;
            _userService=userService;
            _mapper=mapper;
             _blogRepository=blogRepository;
        }
        public Task<PaginatedResult<GetBlogsDto>> Handle(GetBlogsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Blog, bool>> filterExpression = null;
            Func<IQueryable<Blog>, IOrderedQueryable<Blog>> orderByExpression = null;

            //if (!_userService.isUserAuthenticated()) throw new UserVerifyException("User is not authorize");


            if (!string.IsNullOrEmpty(request.Filter))
            {
                filterExpression = entity => entity.User.FirstName.Contains(request.Filter);
            }

            var includeProperties = new Expression<Func<Blog, object>>[]
           {
                e => e.User,
           };

            var dbResult = _blogRepository.GetPaged(request.Page, request.PageSize, request.OrderByPropertname, request.OrderBy, filterExpression, includeProperties);

            PaginatedResult<GetBlogsDto> result = new()
            {
                TotalCount=dbResult.TotalCount,
                TotalPages=dbResult.TotalPages,
            };

            if (dbResult.Data is not null)
            {
                foreach (var data in dbResult.Data)
                {
                    GetBlogsDto getBlogsDto = new()
                    {
                        FullName=data.User?.FirstName+" "+data.User?.LastName,
                        Title=data.Title,
                        Content=data.Content,
                        //ProfilePicture=data.User?.ProfilePicture,
                        Role=Enum.TryParse(data.User?.Role, out Role r) ? r : null,
                        Email=data?.User?.Email,
                        isPublished=data.IsPublished,
                        PublishDate=data.PublishDate,
                        BlogId=data.BlogId,
                        UpdateDate=data.UpdateDate
                        
                    };
                    result.Data.Add(getBlogsDto);
                }
            }
            return Task.FromResult(result);

        }
    }
}
