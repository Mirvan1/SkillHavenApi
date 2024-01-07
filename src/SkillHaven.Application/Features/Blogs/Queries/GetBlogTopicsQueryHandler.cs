using MediatR;
using SkillHaven.Shared.Blog;
using SkillHaven.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Shared.Exceptions;
using SkillHaven.Application.Interfaces.Services;
using Microsoft.Extensions.Localization;
using SkillHaven.Domain.Entities;
using System.Linq.Expressions;
using SkillHaven.Application.Configurations;

namespace SkillHaven.Application.Features.Blogs.Queries
{
    internal class GetBlogTopicsQueryHandler : IRequestHandler<GetBlogTopicsQuery, PaginatedResult<GetBlogTopicsDto>>
    {
        private readonly IBlogTopicRepository _blogTopicRepository;
        private readonly IUserService _userService;
        public readonly IStringLocalizer _localizer;

        public GetBlogTopicsQueryHandler(IBlogTopicRepository blogTopicRepository, IUserService userService )
        {
            _blogTopicRepository=blogTopicRepository;
            _userService=userService;
            _localizer=new Localizer();
        }

        public  Task<PaginatedResult<GetBlogTopicsDto>> Handle(GetBlogTopicsQuery request, CancellationToken cancellationToken)
        {
            if (!_userService.isUserAuthenticated()) throw new UserVerifyException(_localizer["UnAuthorized", "Errors"].Value);


            Expression<Func<BlogTopic, bool>> filterExpression = null;
            Func<IQueryable<BlogTopic>, IOrderedQueryable<BlogTopic>> orderByExpression = null;

            if (!string.IsNullOrEmpty(request.Filter))
            {
                filterExpression = entity => entity.TopicName.Contains(request.Filter) && entity.IsActive==true;
            }

            var includeProperties = new Expression<Func<BlogTopic, object>>[]
          {
                e => e.Blogs,
          };


                var dbResult =  _blogTopicRepository.GetPaged(request.Page, request.PageSize, request.OrderByPropertname, request.OrderBy, filterExpression, includeProperties);
           
            PaginatedResult<GetBlogTopicsDto> result = new()
            {
                TotalCount=dbResult.TotalCount,
                TotalPages=dbResult.TotalPages,
            };

            if(dbResult.Data is not null)
            {
                foreach( var data in dbResult.Data)
                {
                    GetBlogTopicsDto getBlogTopicsDto = new() {     
                    IsActive=data.IsActive,
                    TopicName=data.TopicName,
                    BlogTopicId=data.BlogTopicId
                    };
                    result.Data.Add(getBlogTopicsDto);
                }

            }
            return Task.FromResult(result);
        }
    }
}
