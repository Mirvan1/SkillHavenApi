﻿using SkillHaven.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Interfaces.Repositories
{
    public interface IUserRepository: IRepository<User>
    {
        User GetByEmail(string Email);

        List<User> GetAllSupervisors();
        List<User> GetAllConsultants();

        List<User> ExceptSupervisors();

        List<User> ExceptConsultants();

    }
}
