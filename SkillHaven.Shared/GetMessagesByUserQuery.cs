using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Shared
{
    public class GetMessagesByUserQuery: PaginatedRequest,IRequest<PaginatedResult<GetMessagesByUserDto>>
    {
        public int ReceiverUserId { get; set; }
    }
}
