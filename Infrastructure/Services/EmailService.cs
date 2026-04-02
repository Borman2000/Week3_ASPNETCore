using System.Net.Http.Json;
using Application.Interfaces;
using NotificationAPI.Domain;
using NotificationAPI.Domain.Entities;

namespace Infrastructure.Services;

public class EmailService(IAuthorRepository authorRepo, IHttpClientFactory httpClientFactory) : IEmailService
{
	public async Task SendBookCreatedEmail(string title, Guid authorId) {
		var author = authorRepo.GetByIdAsync(authorId).Result;
		if(author is not null && author.Email is not null)
			await SendEmail(author.Email, subject: $"Book {title} was added to catalog", body: $"Hello, {author.FirstName} {author.LastName}!");
	}

	private async Task SendEmail(string authorEmail, string subject, string body)
	{
		var httpClient = httpClientFactory.CreateClient("NotificationApiService");

		Dictionary<string, string> metadata = new Dictionary<string, string>
		{
			{"subject", subject},
			{"body", body}
		};


		var request = new NotificationRequest
		{
			Email =  authorEmail,
			UserId = Guid.Empty,
			TemplateId = NotificationTemplate.EMPTY_TEMPLATE_ID,
			Type = NotificationType.Transactional,
			Channel =  NotificationChannel.Email,
			Metadata = metadata
		};

		await httpClient.PostAsJsonAsync("/notify", request);
	}
}