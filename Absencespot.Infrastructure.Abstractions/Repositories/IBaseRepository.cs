﻿using Absencespot.Domain.Seedwork;
using Absencespot.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Absencespot.Infrastructure.Abstractions.Repositories
{
    public interface IBaseRepository<T> where T : Entity
    {
        IQueryable<T> AsQueryable(RepositoryOptions? options = null);
        IQueryable<T> Include<TProperty>(IQueryable<T> queryable, Expression<Func<T, TProperty>> navigationPropertyPath);
        IQueryable<T> IncludeThen<TProperty, TSubProperty>(IQueryable<T> queryable, Expression<Func<TProperty, TSubProperty>> navigationPropertyPath);
        Task<IEnumerable<T>> ToListAsync(IQueryable<T> queryable, CancellationToken cancellationToken = default);
        Task<T?> FindByGlobalIdAsync(Guid globalId, RepositoryOptions? options = null, CancellationToken cancellationToken = default);
        Task<T?> FindByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<T?> FirstOrDefaultAsync(IQueryable<T> queryable, CancellationToken cancellationToken = default);
        T Get(Expression<Func<T, bool>> expression);
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAll(Expression<Func<T, bool>> expression);
        T Add(T entity);
        IEnumerable<T> AddRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        T Update(T entity);
        IEnumerable<T> UpdateRange(IEnumerable<T> entities);
        Task<T> GetAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    }
}
