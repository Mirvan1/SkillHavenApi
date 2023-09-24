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
    public class ChatUserRepository : GenericRepository<ChatUser>, IChatUserRepository
    {
        public ChatUserRepository(shDbContext dbContext) : base(dbContext)
        {
        }

        public ChatUser getByUserId(int userId)
        {
            return entity.FirstOrDefault(x => x.UserId==userId);
        }
    }
}
