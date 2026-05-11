using Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BooksAPI.UnitTests.Helpers;

public class MockDb : IDbContextFactory<BookStoreDbContext>
{
    public BookStoreDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<BookStoreDbContext>()
            .UseInMemoryDatabase($"InMemoryTestDb-{DateTime.Now.ToFileTimeUtc()}")
            .Options;

        var mockMediatr = new Mock<IMediator>();

        return new BookStoreDbContext(options, mockMediatr.Object);
    }
}
