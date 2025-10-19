namespace DataAccess.Repositories.Impl;

public interface IRepository<TEntity, TDto> where TEntity:class where TDto : class
{
	Task<IEnumerable<TEntity>> GetAllAsync();
	Task<TEntity?> GetByIdAsync(Guid id);
	Task<TEntity?> AddAsync(TDto dto);
	Task UpdateAsync(TDto dto);
	Task DeleteAsync(Guid id);
    IQueryable<TEntity> Query();
}
