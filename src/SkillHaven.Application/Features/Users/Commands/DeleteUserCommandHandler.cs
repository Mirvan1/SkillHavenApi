using MediatR;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Application.Interfaces.Services;
using SkillHaven.Shared;
using SkillHaven.Shared.Exceptions;
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

        public DeleteUserCommandHandler(IUserRepository userRepository, IUserService userService)
        {
            _userRepository = userRepository;
            _userService=userService;
        }
        public Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            if (!_userService.isUserAuthenticated()) throw new UserVerifyException("User is not authorized");

            var getUser = _userRepository.GetById(request.UserId);

            if (getUser is null || getUser is { IsDeleted: true }) throw new ArgumentNullException("User not found");

            getUser.IsDeleted=true;
            _userRepository.Update(getUser);
            _userRepository.SaveChanges();

            return Task.FromResult(true);

        }
    }
}
