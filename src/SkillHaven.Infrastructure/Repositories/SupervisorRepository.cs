using SkillHaven.Application.Interfaces.Repositories;
using SkillHaven.Domain.Entities;
using SkillHaven.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Infrastructure.Repositories
{
    public class SupervisorRepository : GenericRepository<Supervisor>, ISupervisorRepository
    {
        public SupervisorRepository(shDbContext dbContext) : base(dbContext)
        {
        }


    }
}
