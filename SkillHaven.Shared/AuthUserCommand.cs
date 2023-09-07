using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Shared
{
    public class AuthUserCommand : IRequest<string>
    {
        public required string Token { get;set; }
    }

    //public class UserRefreshToken
    //{
    //    public int UserId { get; set; }
    //    public string FirstName { get; set; }
    //    public string LastName { get; set; }
    //    public string Email { get; set; }

    //    public string RefreshToken { get; set; } = string.Empty;
    //    public DateTime TokenCreated { get; set; }
    //    public DateTime TokenExpires { get; set; }
 
    //}
}
