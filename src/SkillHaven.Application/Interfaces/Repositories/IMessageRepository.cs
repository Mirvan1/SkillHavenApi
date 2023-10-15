using SkillHaven.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Interfaces.Repositories
{
    public interface IMessageRepository : IRepository<Message>
    {
        List<Message> GetMessagesBySender(int senderId);
        List<Message> GetMessagesByReceiver(int receiverId);

    }
}
