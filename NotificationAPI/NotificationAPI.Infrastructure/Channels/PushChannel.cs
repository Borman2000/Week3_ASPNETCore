using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NotificationAPI.Application.DTOs;
using NotificationAPI.Application.Interfaces;
using NotificationAPI.Domain;

namespace NotificationAPI.Infrastructure.Channels;

// TODO: Implement push notifications using e.g. Firebase Cloud Messaging.
public class PushChannel : INotificationChannel
{
    private readonly ILogger<PushChannel> _logger;

    public NotificationChannel ChannelType => NotificationChannel.Push;

    public PushChannel(IConfiguration configuration, ILogger<PushChannel> logger)
    {
        _logger = logger;

        // Initialize Firebase if not already done
//        if (FirebaseApp.DefaultInstance == null)
//        {
//            var credentialPath = configuration["Firebase:CredentialPath"];
//            FirebaseApp.Create(new AppOptions
//            {
//                Credential = GoogleCredential.FromFile(credentialPath)
//            });
//        }
    }

    public async Task<NotificationResult> SendAsync(NotificationRequest request, RenderedTemplate template, NotificationUser user)
    {
        _logger.LogInformation("Preparing to send push notification {NotificationId} to user {UserId}", request.Id, user.Id);

//        if (string.IsNullOrEmpty(user.DeviceToken))
//        {
//            return new NotificationResult
//            {
//                NotificationId = request.Id,
//                Channel = ChannelType,
//                Status = DeliveryStatus.Failed,
//                ErrorMessage = "No device token provided"
//            };
//        }
//
//        try
//        {
//            var message = new Message
//            {
//                Token = user.DeviceToken,
//                Notification = new Notification
//                {
//                    Title = template.Subject,
//                    Body = StripHtml(template.Body)
//                },
//                Data = new Dictionary<string, string>
//                {
//                    ["notificationId"] = request.Id,
//                    ["type"] = request.Type.ToString()
//                },
//                // Platform-specific configuration
//                Android = new AndroidConfig
//                {
//                    Priority = request.Priority == NotificationPriority.Critical
//                        ? Priority.High
//                        : Priority.Normal,
//                    Notification = new AndroidNotification
//                    {
//                        ClickAction = "OPEN_NOTIFICATION"
//                    }
//                },
//                Apns = new ApnsConfig
//                {
//                    Aps = new Aps
//                    {
//                        Sound = "default",
//                        Badge = 1
//                    }
//                }
//            };
//
//            var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
//
//            _logger.LogInformation("Push notification sent for {NotificationId}, MessageId: {MessageId}", request.Id, response);
//
//            return new NotificationResult
//            {
//                NotificationId = request.Id,
//                Channel = ChannelType,
//                Status = DeliveryStatus.Sent,
//                ExternalId = response,
//                ProcessedAt = DateTime.UtcNow
//            };
//        }
//        catch (FirebaseMessagingException ex) when (
//            ex.MessagingErrorCode == MessagingErrorCode.Unregistered)
//        {
//            // Device token is invalid, mark for cleanup
//            _logger.LogWarning(
//                "Invalid device token for notification {NotificationId}",
//                request.Id);
//
//            return new NotificationResult
//            {
//                NotificationId = request.Id,
//                Channel = ChannelType,
//                Status = DeliveryStatus.Unsubscribed,
//                ErrorMessage = "Device token unregistered",
//                ProcessedAt = DateTime.UtcNow
//            };
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex,
//                "Failed to send push notification {NotificationId}",
//                request.Id);
//
//            return new NotificationResult
//            {
//                NotificationId = request.Id,
//                Channel = ChannelType,
//                Status = DeliveryStatus.Failed,
//                ErrorMessage = ex.Message,
//                ProcessedAt = DateTime.UtcNow
//            };
//        }

            return new NotificationResult
            {
                Id = request.Id,
                Channel = ChannelType,
                Status = DeliveryStatus.Failed,
                ErrorMessage = "Not implemented",
                ProcessedAt = DateTime.UtcNow
            };

    }

    private string StripHtml(string html)
    {
        return Regex.Replace(
            html, "<[^>]*>", string.Empty);
    }
}