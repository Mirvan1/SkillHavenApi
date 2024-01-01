using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Shared.User
{
    public class MailUserCheckerCommand:IRequest<bool>
    {
        public string Email { get; set; }
    }
}
