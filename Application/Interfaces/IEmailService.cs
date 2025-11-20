namespace Application.Interfaces;

public interface IEmailService
{
	Task SendBookCreatedEmail(string title, Guid authorId);
}