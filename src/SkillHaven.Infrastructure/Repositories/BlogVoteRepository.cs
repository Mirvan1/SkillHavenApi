using Microsoft.EntityFrameworkCore;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Domain.Entities;
using SkillHaven.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Infrastructure.Repositories
{
    public class BlogVoteRepository : GenericRepository<BlogVote>, IBlogVoteRepository
    {
        public BlogVoteRepository(shDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<int> VotesByBlog(int blogId,CancellationToken ct)
        {
            var voteList= await entity.Where(x => x.BlogId == blogId).ToListAsync(ct);
            int upVotes=voteList.Where(x=>x.VoteValue==true).Count();
            int downVotes= voteList.Where(x => x.VoteValue==false).Count();
            return upVotes-downVotes;
        }

       public async Task<List<BlogVote>> GetByUserId(int userId,int blogId,CancellationToken ct)
        {
            return await entity.Where(x => x.UserId == userId && x.BlogId == blogId).ToListAsync(ct);
        }   
    }
}
