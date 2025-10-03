using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class BooksDb : DbContext
{
    public BooksDb(DbContextOptions<BooksDb> options)
        : base(options) { }

    public DbSet<Book> Books => Set<Book>();
}