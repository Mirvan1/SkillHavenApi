using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Domain.Entities;
using SkillHaven.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace SkillHaven.Infrastructure.Repositories
{
    public class ChatUserConnectionRepository : GenericRepository<ChatUserConnection>, IUserConnectionRepository
    {
        public ChatUserConnectionRepository(shDbContext dbContext) : base(dbContext)
        {
        }

        public bool CheckUserConnected(int userId, string connectionId)
        {
            return entity.Any(x => x.ChatUserId==userId && x.ConnectionId.Equals(connectionId));
        }

        public ChatUserConnection GetByUserId(int userId)
        {
            return entity.FirstOrDefault(x => x.ChatUserId==userId);
        }


        public ChatUserConnection GetByConnnectionId(string connectionId)
        {
            return entity.Include(x=>x.ChatUser)?.FirstOrDefault(x => x.ConnectionId==connectionId);
        }

        public ChatUserConnection GetByChatUserId(int chatUserId)
        {
            return entity.Include(x => x.ChatUser)?.FirstOrDefault(x => x.ChatUserId==chatUserId);
        }


        public void RemoveByConnectionId(string connectionId)
        {
            var userConns= entity.Where(x => x.ConnectionId.Equals(connectionId)).ToList();

            entity.RemoveRange(userConns);
        }
    }
}
