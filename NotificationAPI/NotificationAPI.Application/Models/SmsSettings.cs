namespace NotificationAPI.Application.Models;

public class SmsSettings
{
	public string AccountSid { get; set; } = string.Empty;
	public string AuthToken { get; set; } = string.Empty;
	public string FromNumber { get; set; } = string.Empty;
}