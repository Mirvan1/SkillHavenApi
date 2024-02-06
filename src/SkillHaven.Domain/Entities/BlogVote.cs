using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Domain.Entities
{
    public class BlogVote
    {
        public long BlogVoteId { get; set; }
        public int BlogId { get; set; }
        public int UserId { get; set; }
        public bool VoteValue { get; set; }
        public User User { get; set; }

    }
}
