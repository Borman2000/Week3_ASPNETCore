namespace Application.DTOs;

public class BookCreateDto : BookDto
{
    public BookCreateDto()
    {
        Console.WriteLine("BookCreateDto constructor 1");
    }

    public BookCreateDto(string title, string firstName, string lastName, string isbn, int year, decimal price, string[] categories)
	    : base(title, isbn, year, price, categories)
    {
Console.WriteLine("BookCreateDto constructor 3");
        FirstName = firstName;
        LastName = lastName;
    }

    public string FirstName { get; set; }
    public string LastName { get; set; }
}
