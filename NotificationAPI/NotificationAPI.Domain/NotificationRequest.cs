namespace NotificationAPI.Domain;

public class NotificationRequest
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public Guid UserId { get; set; }
	public NotificationType Type { get; set; }
	public NotificationChannel Channel { get; set; }
	public string TemplateId { get; set; } = string.Empty;
	public Dictionary<string, object> TemplateData { get; set; } = new();
	public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
	public DateTime? ScheduledAt { get; set; }
	public Dictionary<string, string> Metadata { get; set; } = new();
	public string? Email { get; set; }
}