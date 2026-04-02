using Microsoft.AspNetCore.Http;

namespace NotificationAPI.Domain.Interfaces;

public interface INotificationRepository
{
	Task<IResult> CreateAsync(NotificationRequest entity);
	Task<IResult> SaveResultAsync(NotificationResult entity);
	Task<IResult> GetResultsAsync(Guid id);
}
