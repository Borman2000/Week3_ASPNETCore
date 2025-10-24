using Domain.DTOs;
using Domain.Entities;

namespace DataAccess.Repositories.Impl;

public interface IBookRepository : IRepository<Book, BookDto>
{
	Task<StatisticDto> GetStatistics();
	Task<List<BookDto>> Search(string? title, string? author, string? category, decimal? minPrice, decimal? maxPrice);
}
