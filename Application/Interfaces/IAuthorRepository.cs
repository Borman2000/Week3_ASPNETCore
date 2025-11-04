using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Interfaces;

public interface IAuthorRepository : IRepository<Author>
{
	Task<AuthorDto?> GetByIdWithBooksAsync(Guid id);
}
