namespace Application.DTOs;

public class BookCreateDto : BookDto
{
    public BookCreateDto()
    {
    }

    public BookCreateDto(string title, string firstName, string lastName, string isbn, int year, decimal price, string[] categories)
	    : base(title, isbn, year, price, categories)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public string FirstName { get; set; }
    public string LastName { get; set; }
}
