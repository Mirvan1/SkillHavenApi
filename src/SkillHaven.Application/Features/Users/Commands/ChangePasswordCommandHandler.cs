using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using SkillHaven.Application.Configurations;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Domain.Entities;
using SkillHaven.Shared.Infrastructure.Exceptions;
using SkillHaven.Shared.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Features.Users.Commands
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer _localizer;
        public ChangePasswordCommandHandler(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository=userRepository;
            _httpContextAccessor=httpContextAccessor;
            _localizer=new Localizer();
        }

        public Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {

            var getUserEmail = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email).Value;

            var getUser = _userRepository.GetByEmail(getUserEmail);

            if (getUser is null) throw new DatabaseValidationException(_localizer["UserNotFound","Errors"].Value);

            bool passwordValidation = BCrypt.Net.BCrypt.Verify(request.OldPassword, getUser.Password);

            if (!passwordValidation) throw new DatabaseValidationException(_localizer["NotFound", "Errors", "Old Password"].Value);

            if(!request.NewPassword.Equals(request.ConfirmPassword)) throw new AggregateException("New password and ConfirmPassword is not match");

            getUser.Password=BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            _userRepository.Update(getUser);
            int result = _userRepository.SaveChanges();

            return Task.FromResult(result>0);
        }


    }
}
