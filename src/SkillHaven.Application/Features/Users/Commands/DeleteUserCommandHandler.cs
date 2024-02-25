using MediatR;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Localization;
using SkillHaven.Application.Configurations;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Application.Interfaces.Services;
using SkillHaven.Shared.Exceptions;
using SkillHaven.Shared.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Features.Users.Commands
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IStringLocalizer _localizer;
        private readonly ILoggerService<DeleteUserCommandHandler> _logger;


        public DeleteUserCommandHandler(IUserRepository userRepository, IUserService userService, ILoggerService<DeleteUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _userService = userService;
            _localizer = new Localizer();
            _logger = logger;
        }
        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            if (!_userService.isUserAuthenticated()) throw new UserVerifyException(_localizer["UnAuthorized", "Errors"].Value);

            var getUser = await _userRepository.GetByIdAsync(request.UserId,cancellationToken);

            if (getUser is null || getUser is { IsDeleted: true }) throw new ArgumentNullException(_localizer["UserNotFound","Errors"].Value);

            getUser.IsDeleted=true;
            _userRepository.Update(getUser);
            int result=await _userRepository.SaveChangesAsync(cancellationToken);
            _logger.LogInfo($"The user deleted at {DateTime.Now}. User Email:{getUser.Email} ");
            return  result>0;

        }
    }
}
