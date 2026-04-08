using Application.DTOs;
using Xunit.Sdk;

namespace Xunit;

public partial class Assert
{
	public static void Equal(CategoryDto? expected, CategoryDto? actual)
	{
		bool areEqual = expected is not null && actual is not null && expected.Name == actual.Name && expected.Description == actual.Description;
		if(!areEqual)
			throw EqualException.ForMismatchedValues(expected, actual, "Category Dtos are not equal");
	}
}