using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Domain.Entities
{
    public class ChatUser
    {
        public int UserId { get; set; } 
        public DateTime LastSeen { get; set; }
        public string Status { get; set; } 
        public string ProfilePicture { get; set; }
        public virtual ICollection<Message> SentMessages { get; set; }
        public virtual ICollection<Message> ReceivedMessages { get; set; }
        public virtual ICollection<ChatUserConnection> UserConnections { get; set; }

    }
}
