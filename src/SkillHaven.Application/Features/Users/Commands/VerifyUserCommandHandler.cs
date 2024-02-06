using MediatR;
using Microsoft.Extensions.Localization;
using SkillHaven.Application.Configurations;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Shared.Exceptions;
using SkillHaven.Shared.Infrastructure.Exceptions;
using SkillHaven.Shared.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Features.Users.Commands
{
    public class VerifyUserCommandHandler : IRequestHandler<VerifyUserCommand, bool>
    {
        public readonly IUserRepository _userRepository;
        private readonly IStringLocalizer _localizer;


        public VerifyUserCommandHandler(IUserRepository userRepository )
        {
            _userRepository=userRepository;
            _localizer=new Localizer();

        }


        public async Task<bool> Handle(VerifyUserCommand request, CancellationToken cancellationToken)
        {
            if (request?.UserId == null) throw new ArgumentNullException(_localizer["NotFound", "Errors", "UserId"].Value);

            if (request?.MailSendCode == null) throw new ArgumentNullException(_localizer["NotFound", "Errors", "MailCode"].Value);

            var user = await _userRepository.GetByIdAsync(request.UserId,cancellationToken);

            if (user == null) throw new DatabaseValidationException(_localizer["NotFound", "Errors", "User"].Value);

            if (!user.MailConfirmationCode.Equals(request.MailSendCode)) throw new UserVerifyException(_localizer["NotFound", "Errors", "MailCode"].Value);

            user.HasMailConfirm=true;
            _userRepository.Update(user);
            int result =await _userRepository.SaveChangesAsync(cancellationToken);
            return  result>0;
        }
    }
}
