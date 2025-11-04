using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Interfaces;

public interface IBookRepository : IRepository<Book>
{
	Task<StatisticDto> GetStatistics();
	Task<List<Book>> Search(int? page, int? pageSize, string? title, string? author, string? category, decimal? minPrice, decimal? maxPrice);
}
