using DataAccess.Repositories.Impl;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class BookStoreDbContext(DbContextOptions<BookStoreDbContext> options) : DbContext(options), IUnitOfWork
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
	    modelBuilder.Entity<Author>().HasIndex(author => new {author.FirstName, author.LastName}).IsUnique();
	    modelBuilder.Entity<Book>(book => {
				book.HasIndex(e => e.Title).IsUnique();
				book.Property(e => e.Price).HasColumnType("decimal(5, 2)");
		    });
	    modelBuilder.Entity<Category>().HasIndex(category => new {category.Name}).IsUnique();

	    base.OnModelCreating(modelBuilder);
    }

    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Book> Books => Set<Book>();

    public async void Commit()
    {
        await SaveChangesAsync();
    }

    public void Rollback()
    {

        throw new NotImplementedException();
    }
}