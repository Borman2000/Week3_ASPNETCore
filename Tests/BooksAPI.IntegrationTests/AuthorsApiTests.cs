using System.Net;
using System.Net.Http.Json;
using Application.DTOs;
using BooksAPI.IntegrationTests.Configuration;
using BooksAPI.IntegrationTests.Helpers;

namespace BooksAPI.IntegrationTests;

[Collection("ApiTests")]
public class AuthorsApiTests(CustomWebApplicationFactory factory) : BaseTestClient(factory)
{
	[Fact]
	public async Task GetAuthors_Should_Return_Authors_List()
	{
		// Arrange

		// Act
		var httpResponse = await TestHttpClient.GetAsync("/api/v2/authors");
		var result = await httpResponse.Content.ReadFromJsonAsync<List<AuthorDto>>();

		// Assert
		Assert.True(httpResponse.IsSuccessStatusCode);
		Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
		Assert.NotNull(result);
		Assert.Contains(result, a => a.Id == TestValues.AUTHOR_ID_EXISTS);
		Assert.Contains(result, a => a.Id == TestValues.AUTHOR_ID2_EXISTS);
	}

	[Fact]
	public async Task GetAuthorWithBooks_Should_Return_Author_With_Books()
	{
		// Arrange
		Guid authorId = TestValues.AUTHOR_ID2_EXISTS;

		// Act
		var httpResponse = await TestHttpClient.GetAsync($"/api/v2/authors/{authorId}/books");
		var result = await httpResponse.Content.ReadFromJsonAsync<AuthorDto>();

		// Assert
		Assert.True(httpResponse.IsSuccessStatusCode);
		Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
		Assert.NotNull(result);
		Assert.Equal(authorId, result!.Id);
		Assert.Contains(result.BooksRaw, b => b.Id == TestValues.BOOK_ID1_EXISTS);
	}
}