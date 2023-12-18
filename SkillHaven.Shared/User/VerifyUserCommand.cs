using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Shared.User
{
    public class VerifyUserCommand:IRequest<bool>
    {
        public int UserId { get; set; }
        public string MailSendCode { get; set; }
    }
}
