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
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        public MessageRepository(shDbContext dbContext) : base(dbContext)
        {
        }

        public List<Message> GetMessagesByReceiver(int receiverId)
        {
           
            return entity.Where(x => x.ReceiverId==receiverId).ToList();
        }

        public List<Message> GetMessagesBySender(int senderId)
        {
            return entity.Where(x => x.SenderId==senderId).ToList();
        }
    }
}
