using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Impl;

public interface IUnitOfWork : IDisposable
{
    DbSet<Book> Books { get; }
    DbSet<Author> Authors { get; }
    DbSet<Category> Categories { get; }

    void Commit();
    void Rollback();
}