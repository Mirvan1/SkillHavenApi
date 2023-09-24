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
    public class ChatUserConnectionRepository : GenericRepository<ChatUserConnection>, IUserConnectionRepository
    {
        public ChatUserConnectionRepository(shDbContext dbContext) : base(dbContext)
        {
        }

        public bool CheckUserConnected(int userId, string connectionId)
        {
            return entity.Any(x => x.UserId==userId && x.ConnectionId.Equals(connectionId));
        }

        public ChatUserConnection GetByUserId(int userId)
        {
            return entity.FirstOrDefault(x => x.UserId==userId);
        }


        public ChatUserConnection GetByConnnectionId(string connectionId)
        {
            return entity.FirstOrDefault(x => x.ConnectionId==connectionId);
        }


        public void RemoveByConnectionId(string connectionId)
        {
            var userConns= entity.Where(x => x.ConnectionId.Equals(connectionId)).ToList();

            entity.RemoveRange(userConns);
        }
    }
}
