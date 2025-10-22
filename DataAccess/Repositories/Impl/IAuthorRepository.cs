using Domain.DTOs;
using Domain.Entities;

namespace DataAccess.Repositories.Impl;

public interface IAuthorRepository : IRepository<Author, AuthorDto>
{
	Task<AuthorDto?> GetByIdWithBooksAsync(Guid id);
}
