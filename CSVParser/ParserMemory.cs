using System.Globalization;
using Domain.Entities;

namespace CSVParser;

public class ParserMemory : ICsvParser
{
	public IEnumerable<Book> ParseBooks(string csvContent)
	{
		Book book;
		List<Book> result = new List<Book>();

		Span<Range> splitRanges = stackalloc Range[10];	// 10 fields in csv
		ReadOnlySpan<char> spanCsv = csvContent.AsSpan();
		decimal price = 0.0m;
		int year = 1900;
		DateOnly birthDate = new DateOnly();
		Span<char> categorySpan = stackalloc char[50];

		foreach (var spanLine in spanCsv.EnumerateLines())
		{
			if(spanLine.IsEmpty)
				continue;

			spanLine.Split(splitRanges, ";", StringSplitOptions.TrimEntries);

			var validRow =
				!spanLine[splitRanges[0]].IsEmpty &&	// title
				!spanLine[splitRanges[1]].IsEmpty &&	// ISBN
				!spanLine[splitRanges[2]].IsEmpty &&	// first name
				!spanLine[splitRanges[3]].IsEmpty &&	// last name
				DateOnly.TryParse(spanLine[splitRanges[4]], out birthDate) &&	// birthdate
				decimal.TryParse(spanLine[splitRanges[6]], NumberStyles.Number, CultureInfo.InvariantCulture.NumberFormat, out price) &&	// price
				!spanLine[splitRanges[7]].IsEmpty &&	// categories
				int.TryParse(spanLine[splitRanges[8]], out year);	// year

			if (validRow)	// header row (if exists) will be skipped because of invalid
			{

				book = new Book
				{
					Title = spanLine[splitRanges[0]].ToString(),
					Author = new Author(
						spanLine[splitRanges[2]].ToString(),
						spanLine[splitRanges[3]].ToString(),
						birthDate,
						spanLine[splitRanges[9]].ToString(),
						spanLine[splitRanges[5]].ToString()),
					Categories = ParseCategories(spanLine[splitRanges[7]], categorySpan),
					ISBN = spanLine[splitRanges[1]].ToString(),
					Price = price,
					Year = year
				};
				result.Add(book);
			}
		}

		return result;
	}

	private List<Category> ParseCategories(ReadOnlySpan<char> source, Span<char> categoryChar)
	{
		List<Category> result = [];
		char commaChar = ',';
		int counter = 0;
		for (int i= 0; i < source.Length; i++)
		{
			if (source[i] != commaChar)
			{
				categoryChar[counter++] = source[i];
			}
			else
			{
				result.Add(new Category(categoryChar.Slice(0, counter).ToString()));
				categoryChar.Clear();
				counter = 0;
			}
		}
		result.Add(new Category(categoryChar.Slice(0, counter).ToString()));		// add last category

		return result;
	}
}