namespace NotificationAPI.Domain;

public enum DeliveryStatus
{
	Pending,
	Sent,
	Delivered,
	Failed,
	Bounced,
	Unsubscribed
}