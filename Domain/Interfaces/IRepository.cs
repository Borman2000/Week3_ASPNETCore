namespace Domain.Interfaces;

public interface IRepository<TEntity> where TEntity:class
{
	Task<IEnumerable<TEntity>> GetAllAsync();
	Task<TEntity?> GetByIdAsync(Guid id);
	Task<TEntity?> AddAsync(TEntity entity);
	Task AddBulkAsync(IEnumerable<TEntity> entities);
	Task UpdateAsync(TEntity entity);
	Task DeleteAsync(Guid id);
    IQueryable<TEntity> Query();
}
