using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using SkillHaven.Application.Configurations;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Application.Interfaces.Services;
using SkillHaven.Domain.Entities;
using SkillHaven.Shared;
using SkillHaven.Shared.Blog;
using SkillHaven.Shared.Exceptions;
using SkillHaven.Shared.User;
using System;
using System.Collections.Generic;
using System.Data;
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
        public readonly IStringLocalizer _localizer;
        public readonly IUtilService _utilService;
        private readonly IBlogVoteRepository _blogVoteRepository;
        private readonly IBlogTopicRepository _blogTopicRepository;

        public GetBlogsQueryHandler(IHttpContextAccessor httpContextAccessor, IUserService userService, IMapper mapper, IBlogRepository blogRepository, IUtilService utilService, IBlogVoteRepository blogVoteRepository, IBlogTopicRepository blogTopicRepository)
        {
            _httpContextAccessor=httpContextAccessor;
            _userService=userService;
            _mapper=mapper;
            _blogRepository=blogRepository;
            _localizer=new Localizer();
            _utilService=utilService;
            _blogVoteRepository=blogVoteRepository;
            _blogTopicRepository=blogTopicRepository;
        }
        public async Task<PaginatedResult<GetBlogsDto>> Handle(GetBlogsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Blog, bool>> filterExpression = null;
            Func<IQueryable<Blog>, IOrderedQueryable<Blog>> orderByExpression = null;

            if (!_userService.isUserAuthenticated()) throw new UserVerifyException(_localizer["UnAuthorized", "Errors"].Value);

            if (string.IsNullOrEmpty(request.OrderByPropertname))
            {
                request.OrderBy=false;
                request.OrderByPropertname ="PublishDate";
            }

            if (!string.IsNullOrEmpty(request.Filter))
            {
                filterExpression = entity => entity.Content.Contains(request.Filter);
            }

            if (request.BlogTopicId is not null)
            {
                filterExpression = entity => entity.BlogTopicId==request.BlogTopicId;
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
                        UserPhotoPath=data.User.ProfilePicture,
                        Title=data.Title,
                        Content=data.Content,
                        //ProfilePicture=data.User?.ProfilePicture,
                        Role=Enum.TryParse(data.User?.Role, out Role r) ? r : null,
                        Email=data?.User?.Email,
                        isPublished=data.IsPublished,
                        PublishDate=data.PublishDate,
                        BlogId=data.BlogId,
                        UpdateDate=(DateTime)data.UpdateDate,
                        Vote=await _blogVoteRepository.VotesByBlog(data.BlogId,cancellationToken),
                        NOfReading=data.NOfReading,
                        PhotoPath=_utilService.GetPhotoAsBase64(data.PhotoPath),
                        BlogTopicId=(int)data.BlogTopicId,
                        BlogTopicName= _blogTopicRepository.GetById((int)data.BlogTopicId).TopicName
                    };
                    result.Data.Add(getBlogsDto);
                }
             }
            return result;

        }
    }
}
