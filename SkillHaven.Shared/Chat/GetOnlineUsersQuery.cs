using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Shared.Chat
{
    public class GetOnlineUsersQuery : PaginatedRequest, IRequest<PaginatedResult<GetOnlineUsersDto>>
    {
    }
}
