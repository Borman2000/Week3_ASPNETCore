using AuthAPI.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Infrastructure.Repositories;

public class UsersDbContext : IdentityDbContext<ApplicationUser>
{
	public UsersDbContext(DbContextOptions<UsersDbContext> dbContextOptions)
		: base(dbContextOptions)
	{
	}

	/* protected override void OnModelCreating(ModelBuilder modelBuilder)
	 {
	     base.OnModelCreating(modelBuilder);
	     modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
	 }
	*/
}