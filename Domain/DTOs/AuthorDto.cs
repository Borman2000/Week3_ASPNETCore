using Domain.Entities;

namespace Domain.DTOs;

public class AuthorDto : Author
{
	public List<BookDto> BooksRaw {get; set;} = new();
}
