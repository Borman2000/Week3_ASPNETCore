using System.Net;
using System.Net.Http.Json;
using Application.DTOs;
using BooksAPI.IntegrationTests.Configuration;

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
		Assert.Equal(4, result.Count);
	}

	[Fact]
	public async Task GetAuthorWithBooks_Should_Return_Author_With_Books()
	{
		// Arrange
		Guid authorId = Guid.Parse("AB29FC40-CA47-1067-B31D-00DD010662D2");

		// Act
		var httpResponse = await TestHttpClient.GetAsync($"/api/v2/authors/{authorId}/books");
		var result = await httpResponse.Content.ReadFromJsonAsync<AuthorDto>();

		// Assert
		Assert.True(httpResponse.IsSuccessStatusCode);
		Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
		Assert.NotNull(result);
		Assert.Equal(authorId, result.Id);
		Assert.Equal(2, result.BooksRaw.Count);
	}
}