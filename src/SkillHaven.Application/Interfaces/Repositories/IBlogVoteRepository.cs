using SkillHaven.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Interfaces.Repositories
{
    public interface IBlogVoteRepository : IRepository<BlogVote>
    {
        int VotesByBlog(int blogId);

        List<BlogVote> GetByUserId(int userId, int blogId);

    }
}
