using AseShop.Common.Infrastructure.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Application.Interfaces.Services;
using SkillHaven.Domain.Entities;
using SkillHaven.Shared;
using SkillHaven.Shared.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Features.Users.Commands
{
    internal class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, string>
    {
        public readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public readonly IUserService _userService;

 

        public LoginUserCommandHandler(IUserRepository userRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IUserService userService)
        {
            _userRepository=userRepository;
            _configuration = configuration;
            _httpContextAccessor=httpContextAccessor;
            _userService=userService;
        }

        public Task<string> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = _userRepository.GetByEmail(request.Email);//getbyemail ++

            //bool passwordValidation = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

            //if (!passwordValidation) 
            //    throw new DatabaseValidationException("Password is wrong");

            if(!user.Email.Equals(request.Email))
                throw new DatabaseValidationException("Email cannot found");
          
 
            return Task.FromResult(_userService.CreateToken(user));
        }
    


        //private string CreateToken(User user)
        //{
        //    List<Claim> claims = new List<Claim> { new Claim(ClaimTypes.Name, user.FirstName), new Claim(ClaimTypes.Email, user.Email), new Claim(ClaimTypes.Role, user.Role) };
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["secret-key"]));
        //    var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        //    var token = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddDays(3), signingCredentials: cred);
        //    var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        //    return jwt;
        //}




        //storee refresh token in cookie -security issue 
        //private void SetRefreshToken(ref  UserDto userDto)
        //{
        //    string refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        //    var cookie = new CookieOptions
        //    {
        //        HttpOnly=true,
        //        Expires=DateTime.Now.AddDays(3)
        //    };
        // _httpContextAccessor.HttpContext.Response.Cookies.Append("refreshToken", refreshToken, cookie);
        //    _httpContextAccessor.HttpContext.Response.Cookies.Append("tokenExpired", cookie.Expires.ToString(), cookie);

        //    userDto.refreshToken=refreshToken;
        //    userDto.Created=DateTime.Now;
        //    userDto.Expired=DateTime.Now.AddDays(3);
        //   // return cookie;
        //}
       

    }
}
