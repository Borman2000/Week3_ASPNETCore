using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NotificationAPI.Domain;
using NotificationAPI.Domain.Entities;
using NotificationAPI.Domain.Interfaces;

namespace NotificationAPI.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
	protected readonly NotificationDbContext DbContext;
	protected readonly DbSet<Notification> DbSet;
	protected readonly IMapper DtoMapper;

	public NotificationRepository(NotificationDbContext context, IMapper dtoMapper)
	{
		DbContext = context;
		DbSet = context.Notifications;
		DtoMapper = dtoMapper;
	}

	public async Task<IResult> CreateAsync(NotificationRequest entity)
	{
		var notification = DtoMapper.Map<Notification>(entity);
		var result = DbSet.Add(notification).Entity;
		await DbContext.SaveChangesAsync();
		return Results.Ok(result.Id);
	}

	public async Task<IResult> SaveResultAsync(NotificationResult entity)
	{
		var result = await DbSet.FindAsync(entity.Id);
		if (result is null)
		{
			return Results.NotFound();
		}

		DtoMapper.Map(entity, result);
		await DbContext.SaveChangesAsync();
		return Results.Ok(result.Id);
	}

	public async Task<IResult> GetResultsAsync(Guid id)
	{
		var result = await DbSet.FindAsync(id);
		return result is null ? Results.NotFound() : Results.Ok(result);
	}
}