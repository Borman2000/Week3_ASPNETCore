using AuthAPI.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationAPI.Application.DTOs;
using NotificationAPI.Application.Interfaces;
using NotificationAPI.Application.Models;
using NotificationAPI.Domain;
using NotificationAPI.Domain.Interfaces;
using NotificationAPI.Infrastructure.Services;

namespace NotificationAPI.Infrastructure.Channels;

// TODO: Implement SMS notifications using e.g. Twilio.
public class SmsChannel : INotificationChannel
{
    private readonly SmsSettings _settings;
    private readonly ILogger<SmsChannel> _logger;

    public NotificationChannel ChannelType => NotificationChannel.Sms;

    public SmsChannel(IOptions<SmsSettings> settings, ILogger<SmsChannel> logger)
    {
        _settings = settings.Value;
        _logger = logger;

//        TwilioClient.Init(_settings.AccountSid, _settings.AuthToken);
    }

    public async Task<NotificationResult> SendAsync(NotificationRequest request, RenderedTemplate template, NotificationUser user)
    {
        if (string.IsNullOrEmpty(user.PhoneNumber))
        {
            return new NotificationResult
            {
                Id = request.Id,
                Channel = ChannelType,
                Status = DeliveryStatus.Failed,
                ErrorMessage = "No phone number provided"
            };
        }

//        try
//        {
//            // SMS messages should be plain text and concise
//            var body = template.Body;
//            if (body.Length > 1600)
//            {
//                body = body.Substring(0, 1597) + "...";
//            }
//
//            var message = await MessageResource.CreateAsync(
//                to: new Twilio.Types.PhoneNumber(contact.PhoneNumber),
//                from: new Twilio.Types.PhoneNumber(_settings.FromNumber),
//                body: body);
//
//            _logger.LogInformation(
//                "SMS sent to {Phone} for notification {NotificationId}, SID: {Sid}",
//                MaskPhoneNumber(contact.PhoneNumber), request.Id, message.Sid);
//
//            return new NotificationResult
//            {
//                NotificationId = request.Id,
//                Channel = ChannelType,
//                Status = MapTwilioStatus(message.Status),
//                ExternalId = message.Sid,
//                ProcessedAt = DateTime.UtcNow
//            };
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Failed to send SMS for notification {NotificationId}", request.Id);
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

//    private DeliveryStatus MapTwilioStatus(MessageResource.StatusEnum? status)
//    {
//        return status switch
//        {
//            MessageResource.StatusEnum.Queued => DeliveryStatus.Pending,
//            MessageResource.StatusEnum.Sent => DeliveryStatus.Sent,
//            MessageResource.StatusEnum.Delivered => DeliveryStatus.Delivered,
//            MessageResource.StatusEnum.Failed => DeliveryStatus.Failed,
//            MessageResource.StatusEnum.Undelivered => DeliveryStatus.Failed,
//            _ => DeliveryStatus.Pending
//        };
//    }

    private string MaskPhoneNumber(string phone)
    {
        if (phone.Length <= 4) return "****";
        return new string('*', phone.Length - 4) + phone[^4..];
    }
}
