using System.Net;
using System.Net.Http.Json;
using Application.DTOs;
using IntegrationTests.Configuration;
using IntegrationTests.Helpers;

namespace IntegrationTests;

[Collection("ApiTests")]
public class CategoriesApiTests(CustomWebApplicationFactory factory) : BaseTestClient(factory)
{
	[Fact]
	public async Task PostAuthors_Should_Add_New_Author_To_DB()
	{
		// Arrange
		CategoryDto categoryDto = CategoryDtoBuilder
			.Build()
			.WithDefaultData()
			.Create();

		// Act
		var httpResponse = await TestHttpClient.PostAsJsonAsync("/categories", categoryDto);
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