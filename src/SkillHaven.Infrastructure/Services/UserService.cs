using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Domain.Entities;
using SkillHaven.Shared.User;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Interfaces.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private const string EmailClaim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";


        public UserService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, IMapper mapper)
        {
            _configuration=configuration;
            _httpContextAccessor=httpContextAccessor;
            _userRepository=userRepository;
            _mapper=mapper;
        }

        public string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim> { new Claim(ClaimTypes.Name, user.FirstName), new Claim(ClaimTypes.Email, user.Email), new Claim(ClaimTypes.Role, user.Role) };
            string secretKey = _configuration["secret-key"].Replace("{secret-key-value}",
                Environment.GetEnvironmentVariable("secret-key-value", EnvironmentVariableTarget.User));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddDays(3), signingCredentials: cred);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        public bool isUserAuthenticated()
        {
            string EmailClaim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
            var getUserEmail = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == EmailClaim).Value;

            var getUser = _userRepository.GetByEmail(getUserEmail);

            return !(getUser is null);

        }

        public UserDto GetUser()
        {
            var getUserEmail = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == EmailClaim)?.Value;
            return GetUserFromMail(getUserEmail);
        }

        public UserDto GetUserFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);

            var getUserEmail = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == EmailClaim)?.Value;
            return GetUserFromMail(getUserEmail);  
        }

        private UserDto GetUserFromMail(string email)
        {
            var getUser = _userRepository.GetByEmail(email);
            var userDto = _mapper.Map<UserDto>(getUser);
            return userDto;
        }
    }
}
