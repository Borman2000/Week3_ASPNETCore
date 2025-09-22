namespace WebAPI;

using Microsoft.EntityFrameworkCore;

public class BooksDb : DbContext
{
    public BooksDb(DbContextOptions<BooksDb> options)
        : base(options) { }

    public DbSet<Book> Books => Set<Book>();
}