using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Shared.Chat
{
    public class GetOnlineUsersDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime LastSeen { get; set; }
        public string? Status { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
