using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Shared.Skill
{
    public class GetAllSkillerQuery : PaginatedRequest, IRequest<PaginatedResult<SkillerDto>>
    {
        public string? SearchByName { get; set; } = string.Empty;
    }
}
