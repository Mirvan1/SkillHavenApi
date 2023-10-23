using AutoMapper;
using MediatR;
using Microsoft.Extensions.Localization;
using SkillHaven.Application.Configurations;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Application.Interfaces.Services;
using SkillHaven.Shared.Chat;
using SkillHaven.Shared.Exceptions;
using SkillHaven.Shared.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Features.Chat.ChatUser
{
    public class GetChatUserQueryHandler : IRequestHandler<GetChatUserQuery, GetChatUserDto>
    {
        private readonly IChatUserRepository _chatUserRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public readonly IStringLocalizer _localizer;


        public GetChatUserQueryHandler(IChatUserRepository chatUserRepository, IUserService userService, IMapper mapper)
        {
            _chatUserRepository=chatUserRepository;
            _userService=userService;
            _mapper=mapper;
            _localizer=new Localizer();

        }



        public Task<GetChatUserDto> Handle(GetChatUserQuery request, CancellationToken cancellationToken)
        {
             if (!_userService.isUserAuthenticated()) throw new UserVerifyException(_localizer["UnAuthorized", "Errors"].Value);
            
            
            var getUser = _userService.GetUser();

            if (getUser is null) throw new UnauthorizedAccessException("Sonething wrong in authorize");



            if (request.UserId!= getUser.UserId && getUser.Role!=Role.Admin.ToString()) throw new UnauthorizedAccessException("The user is different from auhtorized user");
            var getChatUser = _chatUserRepository.getByUserId(request.UserId);
             var getChatUserDto = _mapper.Map<GetChatUserDto>(getChatUser);


            return Task.FromResult(getChatUserDto);
        }
    }
}
