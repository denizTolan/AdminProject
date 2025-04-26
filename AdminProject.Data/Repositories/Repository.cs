using AdminProject.Data.Context;
using AdminProject.Data.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace AdminProject.Data.Repositories;

public class Repository<TModel,TKey>: IRepository<TModel,TKey>
    where TModel : class,IEntity<TKey>,new()
{
    private readonly ApplicationDbContext _dbContext;
    
    public Repository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public IQueryable<TModel> GetAll()
    {
        return _dbContext.Set<TModel>().AsQueryable();
    }

    public async Task<TModel> GetById(TKey id)
    {
        return await _dbContext.Set<TModel>().AsNoTracking().FirstOrDefaultAsync(p=> p.Id.Equals(id));
    }

    public async Task Add(TModel model)
    {
        await _dbContext.Set<TModel>().AddAsync(model);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(TModel model)
    {
        _dbContext.Set<TModel>().Update(model);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(TKey id)
    {
        var entity = _dbContext.Set<TModel>().Find(id);
        _dbContext.Set<TModel>().Remove(entity);
        await _dbContext.SaveChangesAsync();
    }
}