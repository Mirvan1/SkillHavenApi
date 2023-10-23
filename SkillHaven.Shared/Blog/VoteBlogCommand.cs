using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Shared.Blog
{
    public class VoteBlogCommand : IRequest<int>
    {
        public int BlogId { get; set; }
        public bool isIncreased { get; set; }
    }
}
