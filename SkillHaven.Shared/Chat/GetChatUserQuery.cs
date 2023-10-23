using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Shared.Chat
{
    public class GetChatUserQuery : IRequest<GetChatUserDto>
    {
        public int UserId { get; set; }
    }
}
