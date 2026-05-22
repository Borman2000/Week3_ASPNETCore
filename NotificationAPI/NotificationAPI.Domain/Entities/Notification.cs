using System.ComponentModel.DataAnnotations;

namespace NotificationAPI.Domain.Entities;

public class Notification
{
//request
	public Guid Id { get; set; } = Guid.NewGuid();
	public Guid UserId { get; set; }
	public NotificationType Type { get; set; }
	public NotificationChannel Channel { get; set; }
	[StringLength(20, ErrorMessage = "Template Id must not exceed 20 characters.")]
	public string TemplateId { get; set; } = string.Empty;
	public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
	public DateTime? ScheduledAt { get; set; }
	public Dictionary<string, string> Metadata { get; set; } = new();

// result
	public DeliveryStatus Status { get; set; }
	[StringLength(20, ErrorMessage = "External Id must not exceed 20 characters.")]
	public string? ExternalId { get; set; }
	[StringLength(500, ErrorMessage = "Error message must not exceed 500characters.")]
	public string? ErrorMessage { get; set; }
	public DateTime ProcessedAt { get; set; }
}
