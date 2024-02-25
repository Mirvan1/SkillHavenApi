using MediatR;
using Microsoft.Extensions.Localization;
using SkillHaven.Application.Configurations;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Shared.Exceptions;
using SkillHaven.Shared.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Features.Users.Commands
{
    public class MailUserCheckerCommandHandler : IRequestHandler<MailUserCheckerCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IStringLocalizer _localizer;
        public MailUserCheckerCommandHandler(IUserRepository userRepository)
        {
            _userRepository=userRepository;
            _localizer = new Localizer();
        }

        public Task<bool> Handle(MailUserCheckerCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Email)) throw new UserVerifyException(_localizer["NotNull","Errors","Email"].Value);

            var getUser = _userRepository.GetByEmail(request.Email);

            return Task.FromResult(getUser is null ? true : false);
        }
    }
}
