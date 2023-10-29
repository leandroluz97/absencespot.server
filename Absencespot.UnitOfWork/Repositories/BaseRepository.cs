using Absencespot.Domain.Seedwork;
using Absencespot.Infrastructure.Abstractions.Repositories;
using Absencespot.SqlServer;
using Absencespot.Utils;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Absencespot.UnitOfWork.Repositories
{
    public class BaseRepository<T>  : IBaseRepository<T> where T : Entity
    {
        protected readonly ApplicationDbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public BaseRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public IQueryable<T> AsQueryable(RepositoryOptions? options = null)
        {
            if(options != null && options.Tracking)
            {
                return _dbSet.AsTracking().AsQueryable();
            }
            return _dbSet.AsNoTracking().AsQueryable();
        }

        public IQueryable<T> Include<TProperty>(IQueryable<T> queryable, Expression<Func<T, TProperty>> navigationPropertyPath)
        {
            return queryable.Include(navigationPropertyPath);
        }

        public IQueryable<T> IncludeThen<TProperty, TSubProperty>(IQueryable<T> queryable, Expression<Func<T, TProperty>> navigationPropertyPath, Expression<Func<TProperty, TSubProperty>> subProperty)
        {
           return queryable.Include(navigationPropertyPath).ThenInclude(subProperty);
        }

        public async Task<IEnumerable<T>> ToListAsync<TProperty, TSubProperty>(IQueryable<T> queryable)
        {
            return await queryable.ToListAsync();
        }
        public async Task<T?> FirstOrDefaultAsync<TProperty, TSubProperty>(IQueryable<T> queryable, CancellationToken cancellationToken = default)
        {
            return await queryable.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<T?> FindByGlobalIdAsync(Guid globalId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(new object[] { globalId }, cancellationToken);
        }

        public async Task<T?> FindByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(id, cancellationToken);
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.AddAsync(entity, cancellationToken);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _dbContext.AddRange(entities);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            await _dbContext.AddRangeAsync(entities, cancellationToken);
        }

        public T? Get(Expression<Func<T, bool>> expression)
        {
            return AsQueryable().FirstOrDefault(expression);
        }

        public IEnumerable<T> GetAll()
        {
            return AsQueryable().AsEnumerable();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> expression)
        {
            return AsQueryable().Where(expression).AsEnumerable();
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await AsQueryable().ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
        {
            return await AsQueryable().Where(expression).ToListAsync(cancellationToken);
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
        {
            return await AsQueryable().FirstOrDefaultAsync(expression, cancellationToken);
        }

        public async Task<T?> GetAsync(CancellationToken cancellationToken = default)
        {
            return await AsQueryable().FirstOrDefaultAsync(cancellationToken);
        }

        public void Remove(T entity)
        {
            _dbContext.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbContext.RemoveRange(entities);
        }

        public void Update(T entity)
        {
            _dbContext.Update(entity);
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            _dbContext.UpdateRange(entities);
        }
    }
}
