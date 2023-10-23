using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Shared.Chat
{
    public class GetMessagesByUserDto
    {
        public int MessageId { get; set; }
        public int SenderChatId { get; set; }
        public int SenderUserId { get; set; }
        public int ReceiverChatId { get; set; }
        public int ReceiverUserId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public string? MessageType { get; set; }
        public string? SeenStatus { get; set; }

        public string? SenderProfilePicture { get; set; }

        public string? ReceiverProfilePicture { get; set; }
        public string? SenderStatus { get; set; }

        public string? ReceiverStatus { get; set; }
    }
}
