using Microsoft.EntityFrameworkCore;
using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Domain.Entities;
using SkillHaven.Infrastructure.Data;
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

        public User GetByEmail(string Email)
        {
            return entity.FirstOrDefault(user => user.Email == Email);
        }
    }

}