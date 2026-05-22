using NotificationAPI.Application.DTOs;
using NotificationAPI.Domain;

namespace NotificationAPI.Application.Interfaces;

public interface INotificationChannel
{
	NotificationChannel ChannelType { get; }
	Task<NotificationResult> SendAsync(NotificationRequest request, RenderedTemplate template, NotificationUser user);
}