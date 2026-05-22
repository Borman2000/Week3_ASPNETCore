using Microsoft.EntityFrameworkCore;
using NotificationAPI.Domain.Entities;
using NotificationAPI.Domain.Interfaces;

namespace NotificationAPI.Infrastructure.Repositories;

public class TemplateRepository : ITemplateRepository
{
	protected readonly DbSet<NotificationTemplate> DbSet;

	public TemplateRepository(NotificationDbContext context)
	{
		DbSet = context.NotificationTemplates;
	}

	public async Task<NotificationTemplate?> GetByIdAsync(string templateId)
	{
		return await DbSet.AsNoTracking().FirstOrDefaultAsync(t => t.Id == templateId);
	}

	public async Task<IEnumerable<NotificationTemplate>> GetAllAsync()
	{
		return await DbSet.AsNoTracking().ToListAsync();
	}
}