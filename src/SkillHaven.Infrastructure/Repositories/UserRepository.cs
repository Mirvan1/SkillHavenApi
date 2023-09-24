using Microsoft.EntityFrameworkCore;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Domain.Entities;
using SkillHaven.Infrastructure.Data;
using SkillHaven.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>,IUserRepository
    {
        public UserRepository(shDbContext dbContext) : base(dbContext)
        {
        }

        public List<User> ExceptConsultants()
        {
            return  entity.Where(x => !x.Role.Equals(Role.Consultant)).ToList();
        }

        public List<User> ExceptSupervisors()
        {
            return entity.Where(x => !x.Role.Equals(Role.Supervisor)).ToList();
        }

        public List<User> GetAllConsultants()
        {
            return entity.Where(x => x.Role.Equals(Role.Consultant)).ToList();
        }

        public List<User> GetAllSupervisors()
        {
            return entity.Where(x => x.Role.Equals(Role.Supervisor)).ToList();
        }

        public User GetByEmail(string Email)
        {
            return entity.FirstOrDefault(user => user.Email == Email);
        }
    }

}