using Domain.Entities;

namespace IntegrationTests.Helpers;

public class BookBuilder
{
	public static string DEFAULT_TITLE = "Default Book Title";
	public static string DEFAULT_ISBN = "123-4-4568-9123-4";
	public static decimal DEFAULT_PRICE = 9.99m;
	public static Guid DEFAULT_AUTHOR_ID = Guid.Parse("AAAAAAAA-BBBB-1111-2222-333333333333");

	private Book _book;

	private BookBuilder()
	{
		_book = new Book{Id =  Guid.NewGuid()};
	}

	public static BookBuilder Build()
	{
		return new BookBuilder();
	}

	public Book Create() => _book;

	public BookBuilder WithTitle(string title)
	{
		_book.Title = title;
		return this;
	}

	public BookBuilder WithIsbn(string isbn)
	{
		_book.ISBN = isbn;
		return this;
	}

	public BookBuilder WithPrice(decimal price)
	{
		_book.Price = price;
		return this;
	}

	public BookBuilder WithAuthorId(Guid authorId)
	{
		_book.AuthorId = authorId;
		return this;
	}

	public BookBuilder WithDefaultData()
	{
		_book.Title = DEFAULT_TITLE;
		_book.ISBN = DEFAULT_ISBN;
		_book.Price = DEFAULT_PRICE;
		_book.AuthorId = DEFAULT_AUTHOR_ID;
		return this;
	}
}