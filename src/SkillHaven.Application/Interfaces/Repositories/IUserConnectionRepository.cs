﻿using SkillHaven.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Interfaces.Repositories
{
    public interface IUserConnectionRepository : IRepository<ChatUserConnection>
    {
        ChatUserConnection GetByUserId(int userId);

       bool CheckUserConnected(int userId, string connectionId);

        void RemoveByConnectionId(string connectionId);

        ChatUserConnection GetByConnnectionId(string connectionId);

        public ChatUserConnection GetByChatUserId(int chatUserId);

    }
}
