using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Shared.Chat
{
    public class GetMessagesByUserQuery : PaginatedRequest, IRequest<GetMessageByUserResponse>
    {
        public int ReceiverUserId { get; set; }
    }

    public class GetMessageByUserResponse : PaginatedResult<GetMessagesByUserDto>
    {
        public string? ReceiverUsername { get; set; }
        public string? ReceiverProfilePicture { get; set; }


    }

}
