using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Domain.Entities
{
    public class Message
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public virtual ChatUser? Sender { get; set; }
        public int ReceiverId { get; set; }
        public virtual ChatUser? Receiver { get; set; }
        public string? Content { get; set; }
        public DateTime Timestamp { get; set; }
        public string? MessageType { get; set; } 
        public string? SeenStatus { get; set; }
    }
}
