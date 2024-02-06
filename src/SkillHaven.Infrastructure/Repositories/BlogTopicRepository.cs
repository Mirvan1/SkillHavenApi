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
    public class BlogTopicRepository : GenericRepository<BlogTopic>, IBlogTopicRepository
    {
        public BlogTopicRepository(shDbContext dbContext) : base(dbContext)
        {
        }
    }
}
