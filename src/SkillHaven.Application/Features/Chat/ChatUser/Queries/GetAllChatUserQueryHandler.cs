﻿using MediatR;
using Microsoft.Extensions.Localization;
using SkillHaven.Application.Configurations;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Application.Interfaces.Services;
using SkillHaven.Shared;
using SkillHaven.Shared.Chat;
using SkillHaven.Shared.Exceptions;
using SkillHaven.Shared.User;
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
        public readonly IStringLocalizer _localizer;
        private readonly IUserRepository _userRepository;
        private readonly IUtilService _utilService;

        public GetAllChatUserQueryHandler(IUserConnectionRepository userConnectionRepository, IUserService userService, IChatUserRepository chatUserRepository, IUserRepository userRepository, IUtilService utilService)
        {
            _userConnectionRepository=userConnectionRepository;
            _userService=userService;
            _chatUserRepository=chatUserRepository;
            _localizer=new Localizer();
            _userRepository=userRepository;
            _utilService=utilService;
        }

        public async Task<PaginatedResult<GetChatUserDto>> Handle(GetAllChatUserQuery request, CancellationToken cancellationToken)
        {
            if (!_userService.isUserAuthenticated()) throw new UserVerifyException(_localizer["UnAuthorized", "Errors"].Value);


            var getUser = _userService.GetUser();

            if (getUser is null) throw new UnauthorizedAccessException(_localizer["UserNotFound","Errors"].Value);
            //if (getUser.Role!=Role.Admin.ToString()) throw new UnauthorizedAccessException("Only Admin access this endpoint");

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
                    var userConnection =await _userConnectionRepository.GetByChatUserIdAsync(dbChatUser.Id,cancellationToken);
                    var userInfo =await _userRepository.GetByIdAsync(dbChatUser.UserId, cancellationToken);
                    GetChatUserDto getChatUserDto = new()
                    {
                        Id=dbChatUser.Id,
                        UserId=dbChatUser.UserId,
                        LastSeen=dbChatUser.LastSeen,
                        Status=dbChatUser.Status,
                        ProfilePicture=_utilService.GetPhotoAsBase64(dbChatUser.ProfilePicture),
                        ConnectionId=userConnection?.ConnectionId,
                        ConnectedTime=userConnection?.ConnectedTime,
                        FullName=userInfo!=null ? userInfo.FirstName+" "+userInfo.LastName:""

                    };
                    chatUserDto.Data.Add(getChatUserDto);
                }
            }

            return chatUserDto;
        }

    }
}
