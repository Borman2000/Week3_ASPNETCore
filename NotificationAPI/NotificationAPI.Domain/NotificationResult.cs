namespace NotificationAPI.Domain;

public class NotificationResult
{
	public Guid Id { get; set; }
	public NotificationChannel Channel { get; set; }
	public DeliveryStatus Status { get; set; }
	public string? ExternalId { get; set; }
	public string? ErrorMessage { get; set; }
	public DateTime ProcessedAt { get; set; }
}