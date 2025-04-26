using AdminProject.Data.Domain.Users;

namespace AdminProject.Data.Repositories;

public interface IRepository<TModel,TKey>
    where TModel : class,IEntity<TKey>,new()
{
    IQueryable<TModel> GetAll();
    
    Task<TModel> GetById(TKey id);
    Task Add(TModel model);
    Task Update(TModel model);
    Task Delete(TKey id);
}

public interface IRepository<TModel> : IRepository<TModel, string>
    where TModel : class,IEntity<string>,new()
{
    
}