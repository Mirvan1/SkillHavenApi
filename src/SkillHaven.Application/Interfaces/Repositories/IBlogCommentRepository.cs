using MediatR;
using SkillHaven.Domain.Entities;
using SkillHaven.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Interfaces.Repositories
{
    public interface IBlogCommentRepository:IRepository<BlogComments>
    {
        public PaginatedResult<BlogComments> getCommentsByBlog(int BlogId, int Page, int PageSize, bool OrderBy, string OrderByPropertname); 
    }
}
