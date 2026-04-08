using Application.Interfaces;
using CSVParser;
using Domain.Entities;
using MediatR;

namespace Application.Books.UploadCsvSpan;

public class UploadCsvSpanCommandHandler(IBookRepository bookRepository, ParserMemory csvParser) : IRequestHandler<UploadCsvSpanCommand, Task>
{
	public async Task<Task> Handle(UploadCsvSpanCommand command, CancellationToken cancellationToken)
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