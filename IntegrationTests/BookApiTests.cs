using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Application.Books.Create;
using Application.Books.Update;
using Application.DTOs;
using IntegrationTests.Configuration;
using IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationTests;

public class BookApiTests(CustomWebApplicationFactory factory) : BaseTestCqrs(factory)
{
	// Arrange
	// Act
	// Assert
	[Fact]
	public async Task GetBooksQuery_For_One_Page_With_Two_Books_Should_Return_Books_List()
	{
		// Arrange
//		Book book = BookBuilder
//			.Build()
//			.WithDefaultData()
//			.Create();

		// Act
		var httpResponse = await TestHttpClient.GetAsync("/books?page=1&pageSize=2");
		var result = await httpResponse.Content.ReadFromJsonAsync<List<BookDto?>>();

		// Assert
		Assert.NotNull(result);
		Assert.Equal(2, result.Count);
		Assert.Collection(result, bookDto =>
		{
			Assert.Equal(Guid.Parse("BE61D971-5EBC-4F02-A3A9-6C82895E5C01"), bookDto!.Id);
			Assert.Equal("Fahrenheit 451", bookDto.Title);
		}, todo2 =>
		{
			Assert.Equal(Guid.Parse("BE61D971-5EBC-4F02-A3A9-6C82895E5C02"), todo2!.Id);
			Assert.Equal("Dandelion Wine", todo2.Title);
		});
	}

	[Fact]
	public async Task GetBookByIdQuery_Should_Return_Book_If_Exists()
	{
		// Arrange

		// Act
		var httpResponse = await TestHttpClient.GetAsync($"/books/{TestValues.BOOK_ID1_EXISTS}");
		var result = await httpResponse.Content.ReadFromJsonAsync<BookDto?>();

		// Assert
		Assert.NotNull(result);
		Assert.Equal(TestValues.BOOK_ID1_EXISTS, result.Id);
		Assert.Equal("Fahrenheit 451", result.Title);
	}

	[Fact]
	public async Task GetBookByIdQuery_Should_Return_Null_If_Not_Exists()
	{
		// Arrange

		// Act
		var httpResponse = await TestHttpClient.GetAsync($"/books/{TestValues.BOOK_ID_NOT_EXISTS}");

		// Assert
		Assert.False(httpResponse.IsSuccessStatusCode);
		Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
	}

	[Fact]
	public async Task CreateBookCommand_Should_Create_Book()
	{
		// Arrange
		var request = new CreateBookCommand(TestValues.TITLE, TestValues.ISBN_VALID, TestValues.AUTHOR_ID_EXISTS, 0.99m);

		// Act
		var httpResponse = await TestHttpClient.PostAsJsonAsync("/books", request);
		var result = await httpResponse.Content.ReadAsStringAsync();

		// Assert
		Assert.True(httpResponse.IsSuccessStatusCode);
		Assert.Equal(HttpStatusCode.Created, httpResponse.StatusCode);
		Assert.NotNull(result);
	}

	[Fact]
	public async Task CreateBookCommand_Should_Return_Fail_If_Title_Exists()
	{
		// Arrange
		var request = new CreateBookCommand("Fahrenheit 451", TestValues.ISBN_VALID, TestValues.AUTHOR_ID_EXISTS, 0.99m);

		// Act
		var httpResponse = await TestHttpClient.PostAsJsonAsync("/books", request);

		// Assert
		Assert.False(httpResponse.IsSuccessStatusCode);
		Assert.Equal(HttpStatusCode.InternalServerError, httpResponse.StatusCode);
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData(" ")]
	public async Task CreateBookCommand_Should_Return_Fail_For_Invalid_Title(string title)
	{
		// Arrange
		var request = new CreateBookCommand(title, TestValues.ISBN_VALID, TestValues.AUTHOR_ID_EXISTS, 0.99m);

		// Act
		var httpResponse = await TestHttpClient.PostAsJsonAsync("/books", request);
		string strResult = await httpResponse.Content.ReadAsStringAsync();
		ValidationProblemDetails? result = JsonSerializer.Deserialize<ValidationProblemDetails>(strResult);
//		var result = await Assert.ThrowsAsync<ValidationException>(() => TestHttpClient.PostAsJsonAsync("/books", request));

		// Assert
		Assert.False(httpResponse.IsSuccessStatusCode);
		Assert.NotNull(result);
		Assert.Equal("ValidationFailure", result.Type);
		Assert.True(result.Errors.Count > 0);
		Assert.Equal("'Title' must not be empty.", result.Errors["Title"][0]);
	}

	[Fact]
	public async Task CreateBookCommand_Should_Return_Fail_For_Invalid_ISBN()
	{
		// Arrange
		var request = new CreateBookCommand(TestValues.TITLE, TestValues.ISBN_INVALID, TestValues.AUTHOR_ID_EXISTS, 0.99m);

		// Act
		var httpResponse = await TestHttpClient.PostAsJsonAsync("/books", request);
		string strResult = await httpResponse.Content.ReadAsStringAsync();
		ValidationProblemDetails? result = JsonSerializer.Deserialize<ValidationProblemDetails>(strResult);

		// Assert
		Assert.False(httpResponse.IsSuccessStatusCode);
		Assert.NotNull(result);
		Assert.Equal("ValidationFailure", result.Type);
		Assert.True(result.Errors.Count > 0);
		Assert.Equal("The specified condition was not met for 'ISBN'.", result.Errors["ISBN"][0]);
	}

	[Theory]
	[InlineData("Updated Book Title", 9.99)]
	[InlineData("", 9.99)]
	[InlineData("Updated Book Title", null)]
	public async Task UpdateBookCommand_Should_Update_Data(string? title, double? price)
	{
		// Arrange
		decimal? decimalPrice = (decimal?)price;
		var request = new UpdateBookCommand(TestValues.BOOK_ID2_EXISTS, title, decimalPrice);

		// Act
		var httpResponse = await TestHttpClient.PutAsJsonAsync("/books", request);
		var result = await httpResponse.Content.ReadAsStringAsync();

		// Assert
		Assert.True(httpResponse.IsSuccessStatusCode);
		Assert.Equal(HttpStatusCode.Accepted, httpResponse.StatusCode);
		Assert.NotNull(result);
	}

	[Fact]
	public async Task UpdateBookCommand_Should_Fail_For_Invalid_Price_And_Title()
	{
		// Arrange
		var request = new UpdateBookCommand(TestValues.BOOK_ID2_EXISTS, null, null);

		// Act
		var httpResponse = await TestHttpClient.PutAsJsonAsync("/books", request);
		var strResult = await httpResponse.Content.ReadAsStringAsync();
		ValidationProblemDetails? result = JsonSerializer.Deserialize<ValidationProblemDetails>(strResult);

		// Assert
		Assert.False(httpResponse.IsSuccessStatusCode);
		Assert.NotNull(result);
		Assert.Equal("ValidationFailure", result.Type);
		Assert.True(result.Errors.Count > 0);
	}

	[Fact]
	public async Task UpdateBookCommand_Should_Fail_For_Nonexistent_Book_Id()
	{
		// Arrange
		var request = new UpdateBookCommand(TestValues.BOOK_ID_NOT_EXISTS, "Updated Book Title", 0.99m);

		// Act
		var httpResponse = await TestHttpClient.PutAsJsonAsync("/books", request);

		// Assert
		Assert.False(httpResponse.IsSuccessStatusCode);
		Assert.Equal(HttpStatusCode.InternalServerError, httpResponse.StatusCode);
	}
}