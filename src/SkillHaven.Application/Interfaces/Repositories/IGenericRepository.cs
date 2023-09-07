using SkillHaven.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SkillHaven.Application.Interfaces.Repositories
{
    public interface IRepository<T> where T : class
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);

        PaginatedResult<T> GetPaged(int page, int pageSize, string orderByPropertyName, bool orderBy, Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includeProperties);
        int SaveChanges();
    }
}
