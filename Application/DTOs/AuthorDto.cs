using Domain.Entities;

namespace Application.DTOs;

public class AuthorDto : Author
{
	public List<BookDto> BooksRaw {get; set;} = new();
}
