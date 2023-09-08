using Azure.Core;
using MediatR;
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
    public  class BlogCommentRepository : GenericRepository<BlogComments>, IBlogCommentRepository
    {
        public BlogCommentRepository(shDbContext dbContext) : base(dbContext)
        {
        }

        public PaginatedResult<BlogComments> getCommentsByBlog(int BlogId, int Page, int PageSize, bool OrderBy, string OrderByPropertname)
        {
            Expression<Func<BlogComments, bool>> filterExpression = null;
            Func<IQueryable<BlogComments>, IOrderedQueryable<BlogComments>> orderByExpression = null;

            if (BlogId != null)
            {
                filterExpression = entity => entity.Blog.BlogId==(BlogId);
            }


            var includeProperties = new Expression<Func<BlogComments, object>>[]
              {
                      e=>e.Blog,
                      e => e.User
              };
            var dbResult = GetPaged(Page, PageSize, OrderByPropertname, OrderBy, filterExpression, includeProperties);

            return dbResult;
        }
    }
}
