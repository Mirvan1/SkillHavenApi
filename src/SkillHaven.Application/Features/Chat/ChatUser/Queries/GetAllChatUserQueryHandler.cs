﻿using MediatR;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Application.Interfaces.Services;
using SkillHaven.Shared;
using SkillHaven.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Features.Chat.ChatUser.Queries
{
    public class GetAllChatUserQueryHandler : IRequestHandler<GetAllChatUserQuery, PaginatedResult<GetChatUserDto>>
    {
        private readonly IChatUserRepository _chatUserRepository;
        private readonly IUserService _userService;
        private readonly IUserConnectionRepository _userConnectionRepository;

        public GetAllChatUserQueryHandler(IUserConnectionRepository userConnectionRepository, IUserService userService, IChatUserRepository chatUserRepository)
        {
            _userConnectionRepository=userConnectionRepository;
            _userService=userService;
            _chatUserRepository=chatUserRepository;
        }

        public Task<PaginatedResult<GetChatUserDto>> Handle(GetAllChatUserQuery request, CancellationToken cancellationToken)
        {
           // if (!_userService.isUserAuthenticated()) throw new UserVerifyException("User is not authorize");


            var getUser = _userService.GetUser();

            if (getUser is null) throw new UnauthorizedAccessException("Sonething wrong in authorize");
            if (getUser.Role!=Role.Admin.ToString()) throw new UnauthorizedAccessException("Only Admin access this endpoint");

            Expression<Func<SkillHaven.Domain.Entities.ChatUser, bool>> filterExpression = null;
            Func<IQueryable<SkillHaven.Domain.Entities.ChatUser>, IOrderedQueryable<SkillHaven.Domain.Entities.ChatUser>> orderByExpression = null;
      
            var dbResult = _chatUserRepository.GetPaged(request.Page, request.PageSize, request.OrderByPropertname, request.OrderBy, null, null);

            PaginatedResult<GetChatUserDto> chatUserDto = new()
            {
                TotalCount=dbResult.TotalCount,
                TotalPages=dbResult.TotalPages
            };

            if (dbResult.Data!=null)
            {
                foreach( var dbChatUser in dbResult.Data)
                {
                    var userConnection = _userConnectionRepository.GetByChatUserId(dbChatUser.Id);
                    GetChatUserDto getChatUserDto = new()
                    {
                        Id=dbChatUser.Id,
                        UserId=dbChatUser.UserId,
                        LastSeen=dbChatUser.LastSeen,
                        Status=dbChatUser.Status,
                        ProfilePicture=dbChatUser.ProfilePicture,
                        ConnectionId=userConnection?.ConnectionId,
                        ConnectedTime=userConnection?.ConnectedTime

                    };
                    chatUserDto.Data.Add(getChatUserDto);
                }
            }

            return Task.FromResult(chatUserDto);
        }

    }
}