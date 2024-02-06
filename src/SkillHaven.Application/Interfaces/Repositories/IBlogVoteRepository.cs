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
        Task<int> VotesByBlog(int blogId,CancellationToken ct);

        Task<List<BlogVote>> GetByUserId(int userId, int blogId,CancellationToken ct);

    }
}
