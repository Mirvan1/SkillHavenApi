using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using SkillHaven.Application.Configurations;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Application.Interfaces.Services;
using SkillHaven.Shared.Infrastructure.Exceptions;
using SkillHaven.Shared.User;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Features.Users.Commands
{
    internal class AuthUserCommandHandler : IRequestHandler<AuthUserCommand, string>
    {
        public readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly IStringLocalizer _localizer;
        public AuthUserCommandHandler(IUserRepository userRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor,IUserService userService)
        {
            _userRepository=userRepository;
            _configuration=configuration;
            _httpContextAccessor=httpContextAccessor;
            _userService=userService;
            _localizer=new Localizer();
        }

        public Task<string> Handle(AuthUserCommand request, CancellationToken cancellationToken)
        {
            //check condition for expiration token datetime later

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["secret-key"]); 
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,  
                ValidateAudience = false,
                ValidateLifetime = false  
            };


            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(request.Token, tokenValidationParameters, out securityToken);

            if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            Claim emailClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);

            if (emailClaim is null) throw new AggregateException("Something went wrong ... ");


            var getUser = _userRepository.GetByEmail(emailClaim.Value);

            if (getUser is null) throw new DatabaseValidationException(_localizer["NotFound", "Errors", "User"].Value);


            return Task.FromResult(_userService.CreateToken(getUser));

        }


    }
}
