using NotificationAPI.Domain.Entities;

namespace NotificationAPI.Domain.Interfaces;

public interface ITemplateRepository
{
	Task<NotificationTemplate?> GetByIdAsync(string templateId);
	Task<IEnumerable<NotificationTemplate>> GetAllAsync();
}
