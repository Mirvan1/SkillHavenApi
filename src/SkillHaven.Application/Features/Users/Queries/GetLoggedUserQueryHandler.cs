using AutoMapper;
using MediatR;
using Microsoft.Extensions.Localization;
using SkillHaven.Application.Configurations;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Application.Interfaces.Services;
using SkillHaven.Shared.Exceptions;
using SkillHaven.Shared.Infrastructure.Exceptions;
using SkillHaven.Shared.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Features.Users.Queries
{
    public class GetLoggedUserQueryHandler : IRequestHandler<GetLoggedUserQuery, UserDto>
    {

        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer _localizer;
        private readonly IUserService _userService;

        public GetLoggedUserQueryHandler(IUserRepository userRepository, IMapper mapper,  IUserService userService)
        {
            _userRepository=userRepository;
            _mapper=mapper;
            _localizer=new Localizer(); ;
            _userService=userService;
        }


        public async Task<UserDto> Handle(GetLoggedUserQuery request, CancellationToken cancellationToken)
        {
            if (!_userService.isUserAuthenticated()) throw new UserVerifyException(_localizer["UnAuthorized", "Errors"].Value);

            var getLoggedUser = _userService.GetUser();

            if (getLoggedUser == null)
                throw new DatabaseValidationException(_localizer["NotFound", "Errors", "User"].Value);


            var user = _userRepository.GetById(getLoggedUser.UserId);

            if (user == null)
                throw new DatabaseValidationException(_localizer["NotFound", "Errors", "User"].Value);

            var userDto = _mapper.Map<UserDto>(user);

            return userDto;
        }

    }
}
