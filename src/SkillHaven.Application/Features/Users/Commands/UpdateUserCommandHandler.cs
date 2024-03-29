﻿using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using SkillHaven.Application.Configurations;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Application.Interfaces.Services;
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
    internal class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        public readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer _localizer;
        public UpdateUserCommandHandler(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository=userRepository;
            _httpContextAccessor=httpContextAccessor;
            _localizer = new Localizer();
        }

        public Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var getUserEmail = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email).Value;

            var user = _userRepository.GetByEmail(getUserEmail);

            if (user is null  || user is { IsDeleted: true }) throw new DatabaseValidationException(_localizer["UserNotFound", "Errors"].Value);

            user.Email=request.Email;
            user.FirstName =request.FirstName;
            user.LastName =request.LastName;
            user.ProfilePicture=request.ProfilePicture;

            _userRepository.Update(user);
            int result= _userRepository.SaveChanges();
            return Task.FromResult(result>0);
        }
    }
}
