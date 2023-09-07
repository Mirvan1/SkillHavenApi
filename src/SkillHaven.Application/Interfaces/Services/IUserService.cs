using SkillHaven.Domain.Entities;
using SkillHaven.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Interfaces.Services
{
    public interface IUserService
    {

        public string CreateToken(User user);

        public bool isUserAuthenticated();
        public UserDto GetUser();
    }
}
