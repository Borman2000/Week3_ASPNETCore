using System.Globalization;
using Domain.Entities;

namespace CSVParser;

public class ParserSimple : ICsvParser
{
	public IEnumerable<Book> ParseBooks(string csvContent)
	{
		IEnumerable<Book> result = new List<Book>();
		Book book;

		string[] arrBooks = csvContent.Split("\r\n").Skip(1).ToArray();
		string line;
		decimal price = 0.0m;
		int year = 1900;
		DateOnly birthDate = new DateOnly();

		for (var i = 0; i < arrBooks.Length; i++)
		{
			line = arrBooks[i];
			if (string.IsNullOrWhiteSpace(line))
				continue;

			string[] fields = line.Split(';');

			var validRow =
				!string.IsNullOrWhiteSpace(fields[0]) &&	// title
				!string.IsNullOrWhiteSpace(fields[1]) &&	// ISBN
				!string.IsNullOrWhiteSpace(fields[2]) &&	// first name
				!string.IsNullOrWhiteSpace(fields[3]) &&	// last name
				DateOnly.TryParse(fields[4], out birthDate) &&	// birthdate
				decimal.TryParse(fields[6], NumberStyles.Number, CultureInfo.InvariantCulture.NumberFormat, out price) &&	// price
				!string.IsNullOrWhiteSpace(fields[7]) &&	// categories
			    int.TryParse(fields[8], out year);	// year

			if (validRow)
			{
				book = new Book
				{
					Title = fields[0],
					Author = new Author(fields[2], fields[3], birthDate, fields[9], fields[5]),
					Categories = fields[7].Split(",").Select(c => new Category(c)).ToList(),
					ISBN = fields[1],
					Price = price,
					Year = year
				};
				result = result.Append(book);
			}
		}

		return result;
	}
}