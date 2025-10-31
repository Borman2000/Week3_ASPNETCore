using Domain.DTOs;
using Domain.Entities;

namespace Infrastructure.Repositories.Impl;

public interface IAuthorRepository : IRepository<Author, AuthorDto>
{
	Task<AuthorDto?> GetByIdWithBooksAsync(Guid id);
}
