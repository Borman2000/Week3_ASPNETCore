using Application.Books.Create;
using Application.Books.GetById;
using Application.Books.GetPagedList;
using Application.Books.Update;
using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Events;
using FluentValidation.TestHelper;
using Infrastructure.Repositories;
using Moq;
using UnitTests.Helpers;

namespace UnitTests;

public class BookTests
{
	[Fact]
	public async Task Book_UpdatePrice_Should_Emit_PriceChangedEvent()
	{
		// Arrange
		await using var context = new MockDb().CreateDbContext();
		var mockMapper = new Mock<IMapper>();
		mockMapper.Setup(m => m.Map<BookDto>(It.IsAny<Book>()))
			.Returns((Book b) => new BookDto{Title = b.Title, Id = b.Id, ISBN =  b.ISBN, Price =  b.Price, DomainEvents = b.DomainEvents });

		var bookRepo = new BookRepository(context, mockMapper.Object);
		await bookRepo.AddAsync(new Book {Id = Guid.Parse("AAAAAAAA-BBBB-1111-2222-333333333333"), Title = "Test Book Title", ISBN = "123-4-4568-9123-4", Price = 0.99m});
		await context.SaveChangesAsync();

		var handler = new UpdateBookCommandHandler(bookRepo, mockMapper.Object);
		var request = new UpdateBookCommand(Guid.Parse("AAAAAAAA-BBBB-1111-2222-333333333333"), "Test Book Title", 9.99m);

		// Act
		var result = await handler.Handle(request, CancellationToken.None);

		// Assert
		mockMapper.Verify(m => m.Map<BookDto>(It.IsAny<Book>()), Times.Once);
		Assert.NotNull(result);
		Assert.Equal(9.99m, result.Price);
		Assert.Contains(result.DomainEvents!, e => e is PriceChangedEvent { OldPrice: 0.99m } evt && evt.NewPrice == 9.99m);	// the same as "e is PriceChangedEvent { OldPrice: 0.99m, NewPrice: 9.99m }"
	}

	[Fact]
	public async Task CreateBookCommand_Should_Create_Book()
	{
		// Arrange
		await using var context = new MockDb().CreateDbContext();
		var mockMapper = new Mock<IMapper>();
		mockMapper.Setup(m => m.Map<BookDto>(It.IsAny<Book>()))
			.Returns((Book b) => new BookDto{Title = b.Title, Id = b.Id, ISBN =  b.ISBN, Price =  b.Price});

		var bookRepo = new BookRepository(context, mockMapper.Object);
		var handler = new CreateBookCommandHandler(bookRepo, mockMapper.Object);
		var request = new CreateBookCommand("Test Book Title", "123-4-4568-9123-4", Guid.Parse("AAAAAAAA-BBBB-1111-2222-333333333333"), 0.99m);

		// Act
		var result = await handler.Handle(request, CancellationToken.None);

		// Assert
		mockMapper.Verify(m => m.Map<BookDto>(It.IsAny<Book>()), Times.Once);
		Assert.NotNull(result);
		Assert.Equal("Test Book Title", result.Title);
	}

	[Fact]
	public async Task UpdateBookCommand_Should_Update_Book_Data()
	{
		// Arrange
		await using var context = new MockDb().CreateDbContext();
		var mockMapper = new Mock<IMapper>();
		mockMapper.Setup(m => m.Map<BookDto>(It.IsAny<Book>()))
			.Returns((Book b) => new BookDto{Title = b.Title, Id = b.Id, ISBN =  b.ISBN, Price =  b.Price});

		var bookRepo = new BookRepository(context, mockMapper.Object);
		await bookRepo.AddAsync(new Book {Id = Guid.Parse("AAAAAAAA-BBBB-1111-2222-333333333333"), Title = "Test Book Title", ISBN = "123-4-4568-9123-4", Price = 0.99m});
		await context.SaveChangesAsync();

		var handler = new UpdateBookCommandHandler(bookRepo, mockMapper.Object);
		var request = new UpdateBookCommand(Guid.Parse("AAAAAAAA-BBBB-1111-2222-333333333333"), "Updated Book Title", 9.99m);

		// Act
		var result = await handler.Handle(request, CancellationToken.None);

		// Assert
		mockMapper.Verify(m => m.Map<BookDto>(It.IsAny<Book>()), Times.Once);
		Assert.NotNull(result);
		Assert.Equal("Updated Book Title", result.Title);
		Assert.Equal(9.99m, result.Price);
	}

	[Fact]
	public async Task GetBookByIdQuery_Should_Return_Book_If_Exists()
	{
		// Arrange
		await using var context = new MockDb().CreateDbContext();
		var mockMapper = new Mock<IMapper>();
		mockMapper.Setup(m => m.Map<BookDto>(It.IsAny<Book>()))
			.Returns((Book b) => new BookDto{Title = b.Title, Id = b.Id, ISBN =  b.ISBN, Price =  b.Price});

		var bookRepo = new BookRepository(context, mockMapper.Object);
		await bookRepo.AddAsync(new Book {Id = Guid.Parse("AAAAAAAA-BBBB-1111-2222-333333333333"), Title = "Test Book Title", ISBN = "123-4-4568-9123-4", Price = 0.99m});
		await context.SaveChangesAsync();

		var handler = new GetBookByIdQueryHandler(bookRepo, mockMapper.Object);
		var request = new GetBookByIdQuery(Guid.Parse("AAAAAAAA-BBBB-1111-2222-333333333333"));

		// Act
		var result = await handler.Handle(request, CancellationToken.None);

		// Assert
		mockMapper.Verify(m => m.Map<BookDto>(It.IsAny<Book>()), Times.Once);
		Assert.NotNull(result);
		Assert.Equal(Guid.Parse("AAAAAAAA-BBBB-1111-2222-333333333333"), result.Id);
	}

	[Fact]
	public async Task GetBookByIdQuery_Should_Return_Null_If_Not_Exists()
	{
		// Arrange
		await using var context = new MockDb().CreateDbContext();
		var mockMapper = new Mock<IMapper>();

		BookDto? MapBookToDto(Book? b)
		{
			return b is not null ? new BookDto { Title = b.Title, Id = b.Id, ISBN = b.ISBN, Price = b.Price } : null;
		}

		mockMapper.Setup(m => m.Map<BookDto>(It.IsAny<Book>()))
			.Returns(MapBookToDto);

		var bookRepo = new BookRepository(context, mockMapper.Object);
		await bookRepo.AddAsync(new Book {Id = Guid.Parse("AAAAAAAA-BBBB-1111-2222-333333333333"), Title = "Test Book Title", ISBN = "123-4-4568-9123-4", Price = 0.99m});
		await context.SaveChangesAsync();

		var handler = new GetBookByIdQueryHandler(bookRepo, mockMapper.Object);
		var request = new GetBookByIdQuery(Guid.Parse("BBBBBBBB-AAAA-1111-2222-333333333333"));

		// Act
		var result = await handler.Handle(request, CancellationToken.None);

		// Assert
		mockMapper.Verify(m => m.Map<BookDto>(It.IsAny<Book>()), Times.Once);
		Assert.Null(result);
	}

	[Fact]
	public async Task GetBooksQuery_Should_Return_Books_List()
	{
		// Arrange
		await using var context = new MockDb().CreateDbContext();
		var mockMapper = new Mock<IMapper>();

		List<BookDto> MapBookToDto(List<Book> lst)
		{
			var res = new List<BookDto>();
			foreach (Book b in lst)
				res.Add(new BookDto { Title = b.Title, Id = b.Id, ISBN = b.ISBN, Price = b.Price, Author = b.Author });

			return res;
		}

		mockMapper.Setup(m => m.Map<List<BookDto?>>(It.IsAny<List<Book>>()))
			.Returns(MapBookToDto);

		var authorRepo = new AuthorRepository(context, mockMapper.Object);
		await authorRepo.AddAsync(new Author {Id = Guid.Parse("AAAAAAAA-BBBB-1111-2222-333333333333"), FirstName = "TestFirstName", LastName = "TestLastName"});

		var bookRepo = new BookRepository(context, mockMapper.Object);
		await bookRepo.AddAsync(new Book {Id = Guid.Parse("AAAAAAAA-BBBB-1111-2222-333333333333"), Title = "Test Book Title 1", ISBN = "123-4-4568-9123-4", Price = 0.99m, AuthorId = Guid.Parse("AAAAAAAA-BBBB-1111-2222-333333333333")});
		await bookRepo.AddAsync(new Book {Id = Guid.Parse("CCCCCCCC-DDDD-4444-5555-666666666666"), Title = "Test Book Title 2", ISBN = "987-6-5432-1098-7", Price = 5.99m, AuthorId = Guid.Parse("AAAAAAAA-BBBB-1111-2222-333333333333")});
		await context.SaveChangesAsync();

		var handler = new GetBooksQueryHandler(bookRepo, mockMapper.Object);
		var request = new GetBooksQuery(null, null, null);

		// Act
		var result = await handler.Handle(request, CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(2, result.Count);
		Assert.Collection(result, bookDto =>
		{
			Assert.Equal(Guid.Parse("AAAAAAAA-BBBB-1111-2222-333333333333"), bookDto!.Id);
			Assert.Equal("Test Book Title 1", bookDto.Title);
		}, todo2 =>
		{
			Assert.Equal(Guid.Parse("CCCCCCCC-DDDD-4444-5555-666666666666"), todo2!.Id);
			Assert.Equal("Test Book Title 2", todo2.Title);
		});
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData(" ")]
	public void CreateBookCommandValidator_Should_Throw_Error_For_Invalid_Title(string title)
	{
		// Arrange
		var request = new CreateBookCommand(title, "123-4-4568-9123-4", Guid.Parse("AAAAAAAA-BBBB-1111-2222-333333333333"), 0.99m);
		var validator = new CreateBookCommandValidator();

		// Act
		TestValidationResult<CreateBookCommand> result = validator.TestValidate(request);

		// Assert
		result.ShouldHaveValidationErrorFor(x => x.Title);
	}

	[Theory]
	[InlineData(-1)]
	[InlineData(0)]
	public void CreateBookCommandValidator_Should_Throw_Error_For_Invalid_Price(decimal price)
	{
		// Arrange
		var request = new CreateBookCommand("Test Book Title", "123-4-4568-9123-4", Guid.Parse("AAAAAAAA-BBBB-1111-2222-333333333333"), price);
		var validator = new CreateBookCommandValidator();

		// Act
		TestValidationResult<CreateBookCommand> result = validator.TestValidate(request);

		// Assert
		result.ShouldHaveValidationErrorFor(x => x.Price);
	}

	[Fact]
	public void CreateBookCommandValidator_Should_Throw_Error_For_Invalid_ISBN()
	{
		// Arrange
		var request = new CreateBookCommand("Test Book Title", "123-4-4568-9123-4", Guid.Parse("AAAAAAAA-BBBB-1111-2222-333333333333"), 0.99m);
		var validator = new CreateBookCommandValidator();

		// Act
		TestValidationResult<CreateBookCommand> result = validator.TestValidate(request);

		// Assert
		result.ShouldHaveValidationErrorFor(x => x.ISBN);
	}

	[Fact]
	public void CreateBookCommandValidator_Should_Pass_Validation()
	{
		// Arrange
		var request = new CreateBookCommand("Test Book Title", "978-0-7432-4722-1", Guid.Parse("AAAAAAAA-BBBB-1111-2222-333333333333"), 0.99m);
		var validator = new CreateBookCommandValidator();

		// Act
		TestValidationResult<CreateBookCommand> result = validator.TestValidate(request);

		// Assert
		result.ShouldNotHaveAnyValidationErrors();
	}

	[Fact]
	public void UpdateBookCommandValidator_Should_Throw_Validation_Error()
	{
		// Arrange
		var request = new UpdateBookCommand(Guid.Parse("AAAAAAAA-BBBB-1111-2222-333333333333"), "", 0);
		var validator = new UpdateBookCommandValidator();

		// Act
		TestValidationResult<UpdateBookCommand> result = validator.TestValidate(request);

		// Assert
		result.ShouldHaveValidationErrors();
	}

	[Theory]
	[InlineData("Updated Book Title", 9.99)]
	[InlineData("", 9.99)]
	[InlineData("Updated Book Title", 0)]
	public void UpdateBookCommandValidator_Should_Pass_Validation(string title, decimal price)
	{
		// Arrange
		var request = new UpdateBookCommand(Guid.Parse("AAAAAAAA-BBBB-1111-2222-333333333333"), title, price);
		var validator = new UpdateBookCommandValidator();

		// Act
		TestValidationResult<UpdateBookCommand> result = validator.TestValidate(request);

		// Assert
		result.ShouldNotHaveAnyValidationErrors();
	}
}