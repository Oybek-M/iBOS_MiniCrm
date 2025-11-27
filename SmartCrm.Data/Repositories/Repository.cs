using Microsoft.EntityFrameworkCore;
using SmartCrm.Data.DbContexts;
using SmartCrm.Data.Interfaces;
using SmartCrm.Domain.Entities;
using System.Linq.Expressions;

namespace SmartCrm.Data.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity
    : BaseEntity
{
    private DbSet<TEntity> _dbSet;
    private AppDbContext _appDb;

    public Repository(AppDbContext appDb)
    {
        _dbSet = appDb.Set<TEntity>();
        _appDb = appDb;
    }
    public async Task<bool> Add(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        int result = await _appDb.SaveChangesAsync();

        return result > 0;
    }
    public async Task<bool> AddRange(IEnumerable<TEntity> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        int result = await _appDb.SaveChangesAsync();

        return result > 0;
    }
    public IQueryable<TEntity> GetAll()
    => _dbSet.AsQueryable();

    public async Task<TEntity?> GetById(Guid id)
    {
        TEntity? entity = await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
        {
            return null;
        }

        return entity;
    }

    public async Task<bool> Remove(TEntity entity)
    {
        _dbSet.Remove(entity);
        int result = await _appDb.SaveChangesAsync();

        return result > 0;
    }
    public async Task<bool> Update(TEntity entity)
    {
        _dbSet.Update(entity);
        int result = await _appDb.SaveChangesAsync();

        return result > 0;
    }

    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
            => await _dbSet.FirstOrDefaultAsync(predicate);
}
