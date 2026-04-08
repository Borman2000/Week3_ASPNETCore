using Application.DTOs;

namespace IntegrationTests.Helpers;

public class AuthorDtoBuilder
{
	public static string DEFAULT_FIRST_NAME = "John";
	public static string DEFAULT_LAST_NAME = "Doe";
	public static string DEFAULT_BIO = "John Doe bio";
	public static string DEFAULT_EMAIL = "john.doe@mail.com";
	public static DateOnly DEFAULT_BIRTHDATE = DateOnly.FromDateTime(DateTime.Now);

	private AuthorDto _authorDto;

	private AuthorDtoBuilder()
	{
		_authorDto = new AuthorDto{FirstName = DEFAULT_FIRST_NAME, LastName = DEFAULT_LAST_NAME};
	}

	private AuthorDtoBuilder(Guid id)
	{
		_authorDto = new AuthorDto{Id = id, FirstName = DEFAULT_FIRST_NAME, LastName = DEFAULT_LAST_NAME};
	}

	public static AuthorDtoBuilder Build()
	{
		return new AuthorDtoBuilder();
	}

	public static AuthorDtoBuilder Build(Guid id)
	{
		return new AuthorDtoBuilder(id);
	}

	public AuthorDto Create() => _authorDto;

	public AuthorDtoBuilder WithFirstName(string fName)
	{
		_authorDto.FirstName = fName;
		return this;
	}

	public AuthorDtoBuilder WithLastName(string lName)
	{
		_authorDto.LastName = lName;
		return this;
	}

	public AuthorDtoBuilder WithBirthDate(DateOnly date)
	{
		_authorDto.BirthDate = date;
		return this;
	}

	public AuthorDtoBuilder WithEmail(string email)
	{
		_authorDto.Email = email;
		return this;
	}

	public AuthorDtoBuilder WithBiography(string bio)
	{
		_authorDto.Biography = bio;
		return this;
	}

	public AuthorDtoBuilder WithDefaultData()
	{
		_authorDto.Biography = DEFAULT_BIO;
		_authorDto.Email = DEFAULT_EMAIL;
		_authorDto.BirthDate = DEFAULT_BIRTHDATE;
		return this;
	}
}