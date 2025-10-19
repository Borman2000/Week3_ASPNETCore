using AutoMapper;
using Domain.DTOs;
using Domain.Entities;

namespace DataAccess.Repositories.Impl;

public interface IBookRepository : IRepository<Book, BookDto>
{
	Task<BookDto?> GetByIdAsync(Guid id, IMapper mapper);
}
