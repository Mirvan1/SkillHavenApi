﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Domain.Entities
{
    public class ChatUserConnection
    {
        public int Id { get; set; }
        public int ChatUserId { get; set; }
        public virtual ChatUser ChatUser { get; set; }
        public string ConnectionId { get; set; }
        public DateTime ConnectedTime { get; set; }
    }
}
