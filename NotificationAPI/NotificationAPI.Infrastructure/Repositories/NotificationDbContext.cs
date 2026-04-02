using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NotificationAPI.Domain.Entities;

namespace NotificationAPI.Infrastructure.Repositories;

public class NotificationDbContext : DbContext
{
	public DbSet<Notification> Notifications { get; set; }
	public DbSet<NotificationTemplate> NotificationTemplates { get; set; }

	public NotificationDbContext(DbContextOptions<NotificationDbContext> dbContextOptions) : base(dbContextOptions)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Notification>()
			.Property(b => b.Metadata)
			.HasConversion(
				v => JsonConvert.SerializeObject(v), // Convert Dictionary to JSON string
				v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v) ?? new Dictionary<string, string>() // Convert JSON string back to Dictionary
			);

		modelBuilder.Entity<NotificationTemplate>().HasData(
			new NotificationTemplate{Id = NotificationTemplate.EMPTY_TEMPLATE_ID, Body = "{{ body }}", Subject = "{{ subject }}", IsHtml = false},
			new NotificationTemplate{Id = NotificationTemplate.DEFAULT_TEMPLATE_ID, Body = "Your user ID is {{ Id }}. \nRegistered phone number is {{ PhoneNumber }}, email is {{ Email }}", Subject = "Welcome to VentionTestAPI, {{ FullName }}", IsHtml = false}
			);
	}
}