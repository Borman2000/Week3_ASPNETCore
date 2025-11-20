using Application.Interfaces;

namespace Infrastructure.Services;

public class EmailService(IAuthorRepository authorRepo) : IEmailService
{
	public async Task SendBookCreatedEmail(string title, Guid authorId) {
		var author = authorRepo.GetByIdAsync(authorId).Result;
		if(author is not null && author.Email is not null)
			await SendEmail(author.Email, subject: $"Book {title} was added to catalog", body: $"Hello, {author.FirstName} {author.LastName}!");
	}

	private async Task SendEmail(string authorEmail, string subject, string body)
	{
		// TODO: send email implementation
	}
}