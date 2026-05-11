using System.Net;
using System.Net.Http.Json;
using Application.DTOs;
using BooksAPI.IntegrationTests.Configuration;
using BooksAPI.IntegrationTests.Helpers;

namespace BooksAPI.IntegrationTests;

[Collection("ApiTests")]
public class CategoriesApiTests(CustomWebApplicationFactory factory) : BaseTestClient(factory)
{
	[Fact]
	public async Task PostCategory_Should_Add_New_Category_To_DB()
	{
		// Arrange
		CategoryDto categoryDto = CategoryDtoBuilder
			.Build()
			.WithDefaultData()
			.Create();

		// Act
		var httpResponse = await TestHttpClient.PostAsJsonAsync("/api/v2/categories", categoryDto);
		var result = await httpResponse.Content.ReadFromJsonAsync<CategoryDto>();

		// Assert
		Assert.True(httpResponse.IsSuccessStatusCode);
		Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
		Assert.NotNull(result);
		Assert.Equal(categoryDto, result);
//		Assert.Equal(categoryDto.Name, result!.Name);
//		Assert.Equal(categoryDto.Description, result.Description);
	}
}