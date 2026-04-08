using Application.Interfaces;
using CSVParser;
using Domain.Entities;
using MediatR;

namespace Application.Books.UploadCsv;

public class UploadCsvCommandHandler(IBookRepository bookRepository, ParserSimple csvParser) : IRequestHandler<UploadCsvCommand, Task>
{
	public async Task<Task> Handle(UploadCsvCommand command, CancellationToken cancellationToken)
	{
		var stream = command.file.OpenReadStream();
		stream.Seek(0, SeekOrigin.Begin);
		string result;
// Use a using statement to ensure the StreamReader is properly disposed
		using (StreamReader reader = new StreamReader(stream)) // Specify the correct encoding, e.g., UTF8
		{
			result = reader.ReadToEnd();
		}

		IEnumerable<Book> books = csvParser.ParseBooks(result);
		await bookRepository.AddBulkAsync(books);
		return Task.CompletedTask;
	}
}