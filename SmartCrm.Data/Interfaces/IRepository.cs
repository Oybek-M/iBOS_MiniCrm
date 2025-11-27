using SmartCrm.Domain.Entities;
using System.Linq.Expressions;

namespace SmartCrm.Data.Interfaces;

public interface IRepository<TEntity> where TEntity
    : BaseEntity
{
    Task<bool> Add(TEntity entity);
    Task<bool> AddRange(IEnumerable<TEntity> entities);
    Task<bool> Update(TEntity entity);
    Task<bool> Remove(TEntity entity);
    Task<TEntity?> GetById(Guid id);
    IQueryable<TEntity> GetAll();

    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
}
