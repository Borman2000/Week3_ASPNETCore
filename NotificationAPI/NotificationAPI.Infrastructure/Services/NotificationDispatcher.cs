using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NotificationAPI.Application.DTOs;
using NotificationAPI.Application.Interfaces;
using NotificationAPI.Domain;
using NotificationAPI.Domain.Interfaces;

namespace NotificationAPI.Infrastructure.Services;

public class NotificationDispatcher
{
    private readonly ITemplateService _templateService;
//    private readonly IUserRepository<ApplicationUser, UserDto> _userRepository;
    private readonly IEnumerable<INotificationChannel> _channels;
    private readonly IRateLimiter _rateLimiter;
    private readonly INotificationRepository _repository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<NotificationDispatcher> _logger;

    public NotificationDispatcher(
        ITemplateService templateService,
//        IUserRepository<ApplicationUser, UserDto> userRepository,
        IEnumerable<INotificationChannel> channels,
        IRateLimiter rateLimiter,
        INotificationRepository repository,
        IHttpClientFactory httpClientFactory,
        ILogger<NotificationDispatcher> logger)
    {
        _templateService = templateService;
//        _userRepository = userRepository;
        _channels = channels;
        _rateLimiter = rateLimiter;
        _repository = repository;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<List<NotificationResult>> DispatchAsync(NotificationRequest request)
    {
        var results = new List<NotificationResult>();

        // Get user contact information
//        var user = await _userRepository.GetByIdAsync(request.UserId);
		var user = request.UserId == Guid.Empty && !String.IsNullOrEmpty(request.Email) ? new NotificationUser(request.Email) : GetUserFromApiAsync(request.UserId).Result;
        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found for notification {NotificationId}", request.UserId, request.Id);
            return results;
        }

        string json = JsonConvert.SerializeObject(user);
        var objUser = JsonConvert.DeserializeObject<Dictionary<string, object>>(json)!;
        request.TemplateData = request.TemplateData.Concat(objUser).ToDictionary(k => k.Key, v => v.Value);

        // Render the template
        var template = await _templateService.RenderAsync(request.TemplateId, request.TemplateData);

        // Determine which channels to use
        var channelsToUse = request.Channel == NotificationChannel.All
            ? _channels
            : _channels.Where(c => c.ChannelType == request.Channel);

        foreach (var channel in channelsToUse)
        {
            // Check rate limit for user + channel combination
            var rateLimitKey = $"{request.UserId}:{channel.ChannelType}";
            if (!await _rateLimiter.TryAcquireAsync(rateLimitKey, 10, TimeSpan.FromMinutes(1)))
            {
                _logger.LogWarning("Rate limit exceeded for {Key}, skipping notification", rateLimitKey);
                continue;
            }

            // Create notification record in DB
            await _repository.CreateAsync(request);

            var result = await channel.SendAsync(request, template, user);
            results.Add(result);

            // Store the result for tracking - update created record
            await _repository.SaveResultAsync(result);
        }

        return results;
    }

    private async Task<NotificationUser?> GetUserFromApiAsync(Guid userId)
    {
	    var httpClient = _httpClientFactory.CreateClient("UsersApiService");
	    var response = await httpClient.GetAsync($"/notificationUser/{userId.ToString()}");

	    if (response.IsSuccessStatusCode)
	    {
		    // Deserialize the response content to a DTO (Data Transfer Object)
		    var user = await response.Content.ReadFromJsonAsync<NotificationUser>();
		    return user;
	    }

	    // Handle the error (e.g., throw an exception, return null, log the error)
	    return null;
	    throw new HttpRequestException($"Error calling API: {response.StatusCode}");
    }
}