﻿using Microsoft.EntityFrameworkCore;
using SkillHaven.Application.Interfaces.Repositories;
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
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        private readonly shDbContext _dbContext;
        protected DbSet<T> entity => _dbContext.Set<T>();

        public GenericRepository(shDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region non-async
        public virtual T GetById(int id)
        {
            return _dbContext.Set<T>().Find(id);
        }

        public virtual T GetById(int id, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            var entity = _dbContext.Model.FindEntityType(typeof(T));
            var keyName = entity.FindPrimaryKey().Properties.Select(x => x.Name).Single();

            // Create a lambda to match on primary key
            var parameter = Expression.Parameter(typeof(T), "e");
            var property = Expression.Property(parameter, keyName);
            var constant = Expression.Constant(id);
            var equality = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(equality, parameter);

            return query.FirstOrDefault(lambda);
        }


        public virtual IEnumerable<T> GetAll()
        {
            return _dbContext.Set<T>().ToList();
        }

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _dbContext.Set<T>().Where(predicate);
        }

        public virtual void Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
        }

        public virtual void Update(T entity)
        {
            _dbContext.Set<T>().Update(entity);
        }

        public virtual void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public virtual int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }

        public PaginatedResult<T> GetPaged(int page, int pageSize, string orderByPropertyName, bool orderBy, Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            if (includeProperties!=null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }


            if (!string.IsNullOrEmpty(orderByPropertyName))
            {
                var propertyInfo = typeof(T).GetProperty(orderByPropertyName);
                var parameter = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(parameter, orderByPropertyName);
                Expression conversion = Expression.Convert(property, typeof(Object));

                var lambda = Expression.Lambda<Func<T, Object>>(conversion, parameter);//todo:object may give error
                if (propertyInfo != null)
                {
                    if (orderBy)
                        query = query.OrderBy(lambda.Compile()).AsQueryable();
                    else
                        query = query.OrderByDescending(lambda.Compile()).AsQueryable();

                }
                else
                {
                    query = query.OrderBy(e => propertyInfo.GetValue(e, null)).AsQueryable();
                }



                //// Dönüştürülen sorguyu IOrderedQueryable<TEntity> olarak sakla
                //IOrderedQueryable<T> orderedQuery;

                //if (orderBy)
                //{
                //    orderedQuery = query.OrderBy((dynamic)lambda.Body);
                //}
                //else
                //{
                //    orderedQuery = query.OrderByDescending((dynamic)lambda);
                //}

                //// orderedQuery'yi query'e geri dönüştür
                //query = orderedQuery;
            }
            else
            {
                // Sıralama parametresi belirtilmemişse, varsayılan sıralama yapabilirsiniz
                // Bu örnekte Id sütununa göre sıralanmış varsayılan sıralamadır.
                ;// query = query.OrderBy(entity => entity.);
            }


            //foreach (var includeProperty in includeProperties)
            //{
            //    query = query.(includeProperty);
            //}

            try
            {

                var list = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                int totalCount = query.Count();
                int totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            
            return new PaginatedResult<T>
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                Data = list
            };
            }
            catch (Exception e)
            {
                throw;
            }
        }

        #endregion


        #region async 


        public virtual async Task<T> GetByIdAsync(int id, CancellationToken ct)
        {
            return await _dbContext.Set<T>().FindAsync(id,ct);
        }

        public virtual async Task<T> GetByIdAsync(int id, CancellationToken ct, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            var entity = _dbContext.Model.FindEntityType(typeof(T));
            var keyName = entity.FindPrimaryKey().Properties.Select(x => x.Name).Single();

            // Create a lambda to match on primary key
            var parameter = Expression.Parameter(typeof(T), "e");
            var property = Expression.Property(parameter, keyName);
            var constant = Expression.Constant(id);
            var equality = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(equality, parameter);

            return await query.FirstOrDefaultAsync(lambda,ct);
        }


        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct)
        {
            return await _dbContext.Set<T>().ToListAsync(ct);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync( CancellationToken ct, Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().Where(predicate).ToListAsync(ct);
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct)
        {
            return await _dbContext.Set<T>().Where(predicate).ToListAsync(ct);
        }

        public virtual async void AddAsync(T entity, CancellationToken ct)
        {
            await _dbContext.Set<T>().AddAsync(entity,ct);
        }

        //public virtual void UpdateAsync(T entity, CancellationToken ct)
        //{
        //    _dbContext.Set<T>().Update(entity);
        //}

 
        public virtual async Task<int> SaveChangesAsync(CancellationToken ct)
        {
            return await _dbContext.SaveChangesAsync(ct);
        }

        public async Task<PaginatedResult<T>> GetPagedAsync( CancellationToken ct,int page, int pageSize, string orderByPropertyName, bool orderBy, Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            if (includeProperties!=null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }


            if (!string.IsNullOrEmpty(orderByPropertyName))
            {
                var propertyInfo = typeof(T).GetProperty(orderByPropertyName);
                var parameter = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(parameter, orderByPropertyName);
                Expression conversion = Expression.Convert(property, typeof(Object));

                var lambda = Expression.Lambda<Func<T, Object>>(conversion, parameter);//todo:object may give error
                if (propertyInfo != null)
                {
                    if (orderBy)
                        query = query.OrderBy(lambda.Compile()).AsQueryable();
                    else
                        query = query.OrderByDescending(lambda.Compile()).AsQueryable();

                }
                else
                {
                    query = query.OrderBy(e => propertyInfo.GetValue(e, null)).AsQueryable();
                }

            }
            else
            {
                // Sıralama parametresi belirtilmemişse, varsayılan sıralama yapabilirsiniz
                // Bu örnekte Id sütununa göre sıralanmış varsayılan sıralamadır.
                ;// query = query.OrderBy(entity => entity.);
            }

            try
            {

                var list = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
                int totalCount = query.Count();
                int totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);

                return new PaginatedResult<T>
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    Data = list
                };
            }
            catch (Exception e)
            {
                throw;
            }
        }




        #endregion

    }
}
