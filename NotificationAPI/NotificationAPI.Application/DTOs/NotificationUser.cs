namespace NotificationAPI.Application.DTOs;

public class NotificationUser
{
	public NotificationUser(string email)
	{
		Id = string.Empty;
		Email = email;
	}

	public string Id { get; init; }
	public string Email { get; init; }
	public string? FullName { get; init; }
	public string? PhoneNumber { get; init; }
}