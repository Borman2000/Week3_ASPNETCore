using Microsoft.AspNetCore.Http;

namespace AuthAPI.Domain.Interfaces;

public interface IUserRepository<TEntity, TDto> where TEntity:class where TDto:class
{
	Task<IResult> AddAsync(string login, string password, IList<string>? roles);
	Task<IResult> UpdateAsync(TEntity entity);
	Task<IResult> DeleteAsync(Guid id);
	Task<TEntity?> GetByIdAsync(Guid id);
	Task<IResult> Login(string login, string password);
	Task<IList<TDto>> GetAllAsync();
}
