using AutoMapper;
using MediatR;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Application.Interfaces.Services;
using SkillHaven.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Features.Chat.ChatUser.Queries
{
    public class GetOnlineUsersQueryHandler : IRequestHandler<GetOnlineUsersQuery, PaginatedResult<GetOnlineUsersDto>>
    {
        private readonly IChatUserRepository _chatUserRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public GetOnlineUsersQueryHandler(IChatUserRepository chatUserRepository, IUserService userService, IMapper mapper)
        {
            _chatUserRepository=chatUserRepository;
            _userService=userService;
            _mapper=mapper;
        }

        public Task<PaginatedResult<GetOnlineUsersDto>> Handle(GetOnlineUsersQuery request, CancellationToken cancellationToken)
        {
            // if (!_userService.isUserAuthenticated()) throw new UserVerifyException("User is not authorize");


            var getUser = _userService.GetUser();

            if (getUser is null) throw new UnauthorizedAccessException("Sonething wrong in authorize");
            if (getUser.Role!=Role.Admin.ToString()) throw new UnauthorizedAccessException("Only Admin access this endpoint");

           
            Expression<Func<SkillHaven.Domain.Entities.ChatUser, bool>> filterExpression = entity => entity.Status==ChatUserStatus.Online.ToString();
            Func<IQueryable<SkillHaven.Domain.Entities.ChatUser>, IOrderedQueryable<SkillHaven.Domain.Entities.ChatUser>> orderByExpression = null;

            var dbResult = _chatUserRepository.GetPaged(request.Page, request.PageSize, request.OrderByPropertname, request.OrderBy, filterExpression, null);

            PaginatedResult<GetOnlineUsersDto> paginatedResult = new()
            {
                TotalCount=dbResult.TotalCount,
                TotalPages=dbResult.TotalPages,
                Data=_mapper.Map<List<GetOnlineUsersDto>>(dbResult.Data)
            };

            return Task.FromResult(paginatedResult);
        }


    }

    public enum ChatUserStatus
    {
        Online = 1,
        Offline = 2
    }
}
