using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Shared.User
{
    public class DeleteUserCommand : IRequest<bool>
    {
        public int UserId { get; set; }
    }
}
