using System.Text.RegularExpressions;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using NotificationAPI.Application.DTOs;
using NotificationAPI.Application.Interfaces;
using NotificationAPI.Application.Models;
using NotificationAPI.Domain;

namespace NotificationAPI.Infrastructure.Channels;

public class EmailChannel : INotificationChannel
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailChannel> _logger;

    public NotificationChannel ChannelType => NotificationChannel.Email;

    public EmailChannel(IOptions<EmailSettings> settings, ILogger<EmailChannel> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<NotificationResult> SendAsync(NotificationRequest request, RenderedTemplate template, NotificationUser user)
    {
        if (string.IsNullOrEmpty(user.Email))
        {
            return new NotificationResult
            {
                Id = request.Id,
                Channel = ChannelType,
                Status = DeliveryStatus.Failed,
                ErrorMessage = "No email address provided"
            };
        }

        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
            message.To.Add(new MailboxAddress(user.FullName, user.Email));
            message.Subject = template.Subject;

            // Create the body with both HTML and plain text
            var builder = new BodyBuilder();
            if (template.IsHtml)
            {
                builder.HtmlBody = template.Body;
                builder.TextBody = StripHtml(template.Body);
            }
            else
            {
                builder.TextBody = template.Body;
            }
            message.Body = builder.ToMessageBody();

            // Add tracking headers
            message.Headers.Add("X-Notification-Id", request.Id.ToString());
            message.Headers.Add("X-Notification-Type", request.Type.ToString());

            using var client = new SmtpClient();
//            await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, _settings.UseSsl);
            await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.StartTls);

            if (!string.IsNullOrEmpty(_settings.Username))
            {
                await client.AuthenticateAsync(_settings.Username, _settings.Password);
            }

            var response = await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent to {Email} for notification {NotificationId}", user.Email, request.Id);

            return new NotificationResult
            {
                Id = request.Id,
                Channel = ChannelType,
                Status = DeliveryStatus.Sent,
                ExternalId = response,
                ProcessedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email for notification {NotificationId}", request.Id);

            return new NotificationResult
            {
                Id = request.Id,
                Channel = ChannelType,
                Status = DeliveryStatus.Failed,
                ErrorMessage = ex.Message,
                ProcessedAt = DateTime.UtcNow
            };
        }
    }

    private string StripHtml(string html)
    {
        return Regex.Replace(
            html, "<[^>]*>", string.Empty);
    }
}
