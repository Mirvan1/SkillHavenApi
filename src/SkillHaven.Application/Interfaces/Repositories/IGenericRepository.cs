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

        #region non-async

        T GetById(int id);
        T GetById(int id, params Expression<Func<T, object>>[] includeProperties);
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);

        PaginatedResult<T> GetPaged(int page, int pageSize, string orderByPropertyName, bool orderBy, Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includeProperties);
        int SaveChanges();

        #endregion

        #region async

        Task<T> GetByIdAsync(int id,CancellationToken ct);
        Task<T> GetByIdAsync(int id,CancellationToken ct, params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken ct);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken ct, Expression<Func<T, bool>> predicate);

        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct);
        void AddAsync(T entity, CancellationToken ct);
 
        Task<PaginatedResult<T>> GetPagedAsync(CancellationToken ct,int page, int pageSize, string orderByPropertyName, bool orderBy, Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includeProperties);
        Task<int> SaveChangesAsync(CancellationToken ct);

        #endregion 
    }
}
