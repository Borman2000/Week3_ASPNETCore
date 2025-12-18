using Application.DTOs;

namespace IntegrationTests.Helpers;

public class CategoryDtoBuilder
{
	public static string DEFAULT_CATEGORY_NAME = "Test category";
	public static string DEFAULT_CATEGORY_DESCRIPTION = "Test category description";

	private CategoryDto _categoryDto;

	private CategoryDtoBuilder()
	{
		_categoryDto = new CategoryDto();
	}

	public static CategoryDtoBuilder Build()
	{
		return new CategoryDtoBuilder();
	}

	public CategoryDto Create() => _categoryDto;

	public CategoryDtoBuilder WithName(string catName)
	{
		_categoryDto.Name = catName;
		return this;
	}

	public CategoryDtoBuilder WithDescription(string descr)
	{
		_categoryDto.Description = descr;
		return this;
	}

	public CategoryDtoBuilder WithDefaultData()
	{
		_categoryDto.Name = DEFAULT_CATEGORY_NAME;
		_categoryDto.Description = DEFAULT_CATEGORY_DESCRIPTION;
		return this;
	}
}