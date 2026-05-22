namespace NotificationAPI.Domain;

public enum NotificationType
{
	Transactional,  // Order confirmations, password resets
	Marketing,      // Promotional content
	Alert,          // System alerts, warnings
	Reminder        // Scheduled reminders
}