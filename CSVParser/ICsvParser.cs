using Domain.Entities;

namespace CSVParser;

public interface ICsvParser
{
	IEnumerable<Book> ParseBooks(string csvContent);
}