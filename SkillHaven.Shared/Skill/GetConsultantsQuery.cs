using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Shared.Skill
{
    public class GetConsultantsQuery : PaginatedRequest, IRequest<PaginatedResult<SkillerDto>>
    {
    }
}
